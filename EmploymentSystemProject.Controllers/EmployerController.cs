using EmploymentSystemProject.DSL;
using EmploymentSystemProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Controllers
{
    [Authorize(Roles = "Employer")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployerController : ControllerBase
    {
        private readonly EmployerDSL _employerService;

        public EmployerController(EmployerDSL employerService)
        {
            _employerService = employerService;
        }

        [HttpPost("CreateVacancy")]
        public async Task<IActionResult> CreateVacancy([FromBody] VacancyDTO vacancy)
        {
            await _employerService.CreateVacancyAsync(vacancy);
            return Ok("Vacancy created successfully");
        }

        [HttpPut("UpdateVacancy/{id}")]
        public async Task<IActionResult> UpdateVacancy(int id, [FromBody] VacancyDTO vacancy)
        {
            await _employerService.UpdateVacancyAsync(id, vacancy);
            return Ok("Vacancy updated successfully");
        }

        [HttpDelete("DeleteVacancy/{id}")]
        public async Task<IActionResult> DeleteVacancy(int id)
        {
            await _employerService.DeleteVacancyAsync(id);
            return Ok("Vacancy deleted successfully");
        }

        [HttpGet("GetVacancies")]
        public async Task<ActionResult<IEnumerable<VacancyDTO>>> GetVacancies()
        {
            var vacancies = await _employerService.GetVacanciesAsync();
            return Ok(vacancies);
        }

        [HttpPut("PostVacancy/{id}")]
        public async Task<IActionResult> PostVacancy(int id)
        {
            await _employerService.PostVacancyAsync(id);
            return Ok("Vacancy posted successfully");
        }

        [HttpPut("DeactivateVacancy/{id}")]
        public async Task<IActionResult> DeactivateVacancy(int id)
        {
            await _employerService.DeactivateVacancyAsync(id);
            return Ok("Vacancy deactivated successfully");
        }

        [HttpGet("GetApplicants/{vacancyId}")]
        public async Task<ActionResult<IEnumerable<ApplicantDTO>>> GetApplicants(int vacancyId)
        {
            var applicants = await _employerService.GetApplicantsAsync(vacancyId);
            return Ok(applicants);
        }
    }
}
