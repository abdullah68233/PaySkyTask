using EmploymentSystemProject.Entities;
using EmploymentSystemProject.Helpers;
using EmploymentSystemProject.Repo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmploymentSystemProject.DSL
{
    public class EmployerDSL
    {
        private readonly EmployerRepo _employerRepo;

        public EmployerDSL(EmployerRepo employerRepo)
        {
            _employerRepo = employerRepo;
        }

        public async Task CreateVacancyAsync(VacancyDTO vacancyDTO)
        {
            File_Logger.WriteToLogFile(ActionType.Information, "CreateVacancyAsync method called.");

            if (vacancyDTO == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, "Vacancy details are null.");
                throw new ArgumentNullException(nameof(vacancyDTO), "Vacancy details must be provided.");
            }

            if (vacancyDTO.MaxApplications <= 0)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, "MaxApplications is less than or equal to zero.");
                throw new ArgumentException("Maximum number of applications must be greater than zero.");
            }

            if (vacancyDTO.ExpiryDate <= DateTime.UtcNow)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, "Expiry date is in the past.");
                throw new ArgumentException("Expiry date must be in the future.");
            }

            try
            {
                await _employerRepo.AddAsync(vacancyDTO);
                File_Logger.WriteToLogFile(ActionType.Action, $"Vacancy created successfully: {vacancyDTO.Title}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while creating a vacancy.");
                throw;
            }
        }

        public async Task UpdateVacancyAsync(int id, VacancyDTO vacancyDTO)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"UpdateVacancyAsync method called for Vacancy ID: {id}");

            if (vacancyDTO == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, "Vacancy details are null.");
                throw new ArgumentNullException(nameof(vacancyDTO), "Vacancy details must be provided.");
            }

            var existingVacancy = await _employerRepo.GetByIdAsync(id);
            if (existingVacancy == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {id}");
                throw new KeyNotFoundException("Vacancy not found.");
            }

            try
            {
                await _employerRepo.UpdateAsync(id, vacancyDTO);
                File_Logger.WriteToLogFile(ActionType.Action, $"Vacancy updated successfully: {vacancyDTO.Title}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while updating a vacancy.");
                throw;
            }
        }

        public async Task DeleteVacancyAsync(int id)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"DeleteVacancyAsync method called for Vacancy ID: {id}");

            var existingVacancy = await _employerRepo.GetByIdAsync(id);
            if (existingVacancy == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {id}");
                throw new KeyNotFoundException("Vacancy not found.");
            }

            try
            {
                await _employerRepo.DeleteAsync(id);
                File_Logger.WriteToLogFile(ActionType.Action, $"Vacancy deleted successfully: ID {id}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while deleting a vacancy.");
                throw;
            }
        }

        public async Task<IEnumerable<VacancyDTO>> GetVacanciesAsync()
        {
            File_Logger.WriteToLogFile(ActionType.Information, "GetVacanciesAsync method called.");

            try
            {
                var vacancies = await _employerRepo.GetAllAsync();
                File_Logger.WriteToLogFile(ActionType.Action, "Retrieved vacancies successfully.");
                return vacancies;
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while retrieving vacancies.");
                throw;
            }
        }

        public async Task PostVacancyAsync(int id)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"PostVacancyAsync method called for Vacancy ID: {id}");

            var vacancy = await _employerRepo.GetByIdAsync(id);
            if (vacancy == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {id}");
                throw new KeyNotFoundException("Vacancy not found.");
            }

            try
            {
                await _employerRepo.PostVacancyAsync(id);
                File_Logger.WriteToLogFile(ActionType.Action, $"Vacancy posted successfully: ID {id}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while posting a vacancy.");
                throw;
            }
        }

        public async Task DeactivateVacancyAsync(int id)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"DeactivateVacancyAsync method called for Vacancy ID: {id}");

            var vacancy = await _employerRepo.GetByIdAsync(id);
            if (vacancy == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {id}");
                throw new KeyNotFoundException("Vacancy not found.");
            }

            try
            {
                await _employerRepo.DeactivateVacancyAsync(id);
                File_Logger.WriteToLogFile(ActionType.Action, $"Vacancy deactivated successfully: ID {id}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while deactivating a vacancy.");
                throw;
            }
        }

        public async Task<IEnumerable<ApplicantDTO>> GetApplicantsAsync(int vacancyId)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"GetApplicantsAsync method called for Vacancy ID: {vacancyId}");

            var vacancy = await _employerRepo.GetByIdAsync(vacancyId);
            if (vacancy == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {vacancyId}");
                throw new KeyNotFoundException("Vacancy not found.");
            }

            try
            {
                var applicants = await _employerRepo.GetApplicantsByVacancyIdAsync(vacancyId);
                File_Logger.WriteToLogFile(ActionType.Action, $"Applicants retrieved successfully for Vacancy ID: {vacancyId}");
                return applicants;
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while retrieving applicants.");
                throw;
            }
        }
    }
}
