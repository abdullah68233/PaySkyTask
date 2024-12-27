using EmploymentSystemProject.Entities;
using EnploymentSystemProject.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Repo
{
    public class EmployerRepo
    {
        private readonly EmploymentDbContext _context;

        public EmployerRepo(EmploymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VacancyDTO vacancyDTO)
        {
            var vacancy = new Vacancy
            {
                Title = vacancyDTO.Title,
                Description = vacancyDTO.Description,
                MaxApplications = vacancyDTO.MaxApplications,
                ExpiryDate = vacancyDTO.ExpiryDate,
                IsActive = true // Default to active when created
            };

            await _context.Vacancies.AddAsync(vacancy);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, VacancyDTO vacancyDTO)
        {
            var existingVacancy = await GetByIdAsync(id);
            if (existingVacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            existingVacancy.Title = vacancyDTO.Title ?? existingVacancy.Title;
            existingVacancy.Description = vacancyDTO.Description ?? existingVacancy.Description;
            existingVacancy.MaxApplications = vacancyDTO.MaxApplications > 0 ? vacancyDTO.MaxApplications : existingVacancy.MaxApplications;
            existingVacancy.ExpiryDate = vacancyDTO.ExpiryDate > DateTime.UtcNow ? vacancyDTO.ExpiryDate : existingVacancy.ExpiryDate;

            _context.Vacancies.Update(existingVacancy);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vacancy = await GetByIdAsync(id);
            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VacancyDTO>> GetAllAsync()
        {
            return await _context.Vacancies
                .Select(v => new VacancyDTO
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    MaxApplications = v.MaxApplications,
                    ExpiryDate = v.ExpiryDate,
                    IsActive = v.IsActive
                })
                .ToListAsync();
        }

        public async Task<Vacancy> GetByIdAsync(int id)
        {
            return await _context.Vacancies.FindAsync(id);
        }

        public async Task PostVacancyAsync(int id)
        {
            var vacancy = await GetByIdAsync(id);
            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            if (vacancy.IsActive)
                throw new InvalidOperationException("Vacancy is already active.");

            vacancy.IsActive = true;
            _context.Vacancies.Update(vacancy);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateVacancyAsync(int id)
        {
            var vacancy = await GetByIdAsync(id);
            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            if (!vacancy.IsActive)
                throw new InvalidOperationException("Vacancy is already inactive.");

            vacancy.IsActive = false;
            _context.Vacancies.Update(vacancy);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicantDTO>> GetApplicantsByVacancyIdAsync(int vacancyId)
        {
            return await _context.Applications
                .Where(a => a.VacancyId == vacancyId)
                .Select(a => new ApplicantDTO
                {
                    Id = a.ApplicantId,
                    FullName = a.Applicant.FullName,
                    Email = a.Applicant.Email,
                    PhoneNumber = a.Applicant.PhoneNumber,
                    ApplicationDate = a.ApplicationDate
                })
                .ToListAsync();
        }

        public async Task ArchiveExpiredVacanciesAsync()
        {
            var expiredVacancies = await _context.Vacancies
                .Where(v => v.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredVacancies.Any())
            {
                var archivedVacancies = expiredVacancies.Select(v => new ArchivedVacancy
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    MaxApplications = v.MaxApplications,
                    ExpiryDate = v.ExpiryDate,
                    ArchivedDate = DateTime.UtcNow
                });

                await _context.ArchivedVacancies.AddRangeAsync(archivedVacancies);
                _context.Vacancies.RemoveRange(expiredVacancies);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VacancyDTO>> GetActiveVacanciesAsync()
        {
            return await _context.Vacancies
                .Where(v => v.ExpiryDate > DateTime.UtcNow)
                .Select(v => new VacancyDTO
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    MaxApplications = v.MaxApplications,
                    ExpiryDate = v.ExpiryDate,
                    IsActive = v.IsActive
                })
                .ToListAsync();
        }
    }
}
