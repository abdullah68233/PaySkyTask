using EmploymentSystemProject.DSL;
using EmploymentSystemProject.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly ApplicantDSL _applicantService;

        public ApplicantController(ApplicantDSL applicantService)
        {
            _applicantService = applicantService;
        }

        [HttpGet("SearchVacancies")]
        public async Task<ActionResult<IEnumerable<VacancyDTO>>> SearchVacancies([FromQuery] string title, [FromQuery] DateTime? expiryDate)
        {
            var vacancies = await _applicantService.SearchVacanciesAsync(title, expiryDate);
            return Ok(vacancies);
        }

        [HttpPost("Apply/{vacancyId}")]
        public async Task<IActionResult> ApplyForVacancy(int vacancyId, [FromBody] ApplicantDTO applicant)
        {
            await _applicantService.ApplyForVacancyAsync(vacancyId, applicant);
            return Ok("Application submitted successfully.");
        }
    }
}

