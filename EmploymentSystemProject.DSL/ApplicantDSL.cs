using EmploymentSystemProject.Entities;
using EmploymentSystemProject.Exceptions;
using EmploymentSystemProject.Helpers;
using EmploymentSystemProject.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmploymentSystemProject.DSL
{
    public class ApplicantDSL
    {
        private readonly ApplicantRepo _applicantRepo;

        public ApplicantDSL(ApplicantRepo applicantRepo)
        {
            _applicantRepo = applicantRepo;
        }

        public async Task<IEnumerable<VacancyDTO>> SearchVacanciesAsync(string title, DateTime? expiryDate)
        {
            File_Logger.WriteToLogFile(ActionType.Information, "SearchVacanciesAsync method called.");

            try
            {
                var results = await _applicantRepo.SearchVacanciesAsync(title, expiryDate);
                File_Logger.WriteToLogFile(ActionType.Action, "Vacancies retrieved successfully.");
                return results;
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, "Error occurred while searching for vacancies.");
                throw;
            }
        }

        public async Task ApplyForVacancyAsync(int vacancyId, ApplicantDTO applicant)
        {
            File_Logger.WriteToLogFile(ActionType.Information, $"ApplyForVacancyAsync method called for Vacancy ID: {vacancyId}, Applicant ID: {applicant?.Id}");

            if (applicant == null)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, "Applicant details are null.");
                throw new CustomValidationException("Applicant details must be provided.");
            }

            try
            {
                var vacancy = await _applicantRepo.GetVacancyByIdAsync(vacancyId);
                if (vacancy == null)
                {
                    File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy not found for ID: {vacancyId}");
                    throw new KeyNotFoundException("Vacancy not found.");
                }

                if (!vacancy.IsActive)
                {
                    File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy is inactive: {vacancyId}");
                    throw new InvalidOperationException("Cannot apply for an inactive vacancy.");
                }

                if (vacancy.ExpiryDate <= DateTime.UtcNow)
                {
                    File_Logger.WriteToLogFile(ActionType.Exception, $"Vacancy is expired: {vacancyId}");
                    throw new InvalidOperationException("Cannot apply for an expired vacancy.");
                }

                var applications = await _applicantRepo.GetApplicationsByVacancyIdAsync(vacancyId);
                if (applications.Count() >= vacancy.MaxApplications)
                {
                    File_Logger.WriteToLogFile(ActionType.Exception, $"Max applications reached for Vacancy ID: {vacancyId}");
                    throw new InvalidOperationException("The maximum number of applications has been reached for this vacancy.");
                }

                var recentApplications = await _applicantRepo.GetRecentApplicationsAsync(applicant.Id, TimeSpan.FromHours(24));
                if (recentApplications.Any())
                {
                    File_Logger.WriteToLogFile(ActionType.Exception, $"Applicant ID {applicant.Id} already applied within the last 24 hours.");
                    throw new InvalidOperationException("You can only apply for one vacancy within a 24-hour period.");
                }

                await _applicantRepo.ApplyForVacancyAsync(vacancyId, applicant);
                File_Logger.WriteToLogFile(ActionType.Action, $"Applicant ID {applicant.Id} successfully applied for Vacancy ID: {vacancyId}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, $"Error occurred while applying for Vacancy ID: {vacancyId}");
                throw;
            }
        }
    }
}
