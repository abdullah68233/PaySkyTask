using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int VacancyId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public Vacancy Vacancy { get; set; }
        public Applicant Applicant { get; set; }
    }
}
