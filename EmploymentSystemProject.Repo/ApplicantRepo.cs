using EmploymentSystemProject.Entities;
using EnploymentSystemProject.Core;
using Microsoft.EntityFrameworkCore;

namespace EmploymentSystemProject.Repo
{
    public class ApplicantRepo
    {
        private readonly EmploymentDbContext _context;

        public ApplicantRepo(EmploymentDbContext context)
        {
            _context = context;
        }

        // Search Vacancies with optional filters
        public async Task<IEnumerable<VacancyDTO>> SearchVacanciesAsync(string title, DateTime? expiryDate)
        {
            var query = _context.Vacancies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(v => EF.Functions.Like(v.Title, $"%{title}%"));

            if (expiryDate.HasValue)
                query = query.Where(v => v.ExpiryDate >= expiryDate.Value);

            return await query
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

        // Get Vacancy by ID
        public async Task<Vacancy> GetVacancyByIdAsync(int id)
        {
            return await _context.Vacancies.FindAsync(id);
        }

        public async Task<IEnumerable<Application>> GetApplicationsByVacancyIdAsync(int vacancyId)
        {
            return await _context.Applications
                .Where(a => a.VacancyId == vacancyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetRecentApplicationsAsync(int applicantId, TimeSpan within)
        {
            var thresholdDate = DateTime.UtcNow.Subtract(within);
            return await _context.Applications
                .Where(a => a.ApplicantId == applicantId && a.ApplicationDate >= thresholdDate)
                .ToListAsync();
        }

        public async Task ApplyForVacancyAsync(int vacancyId, ApplicantDTO applicant)
        {
            var application = new Application
            {
                VacancyId = vacancyId,
                ApplicantId = applicant.Id,
                ApplicationDate = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }
    }
}
