using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Entities
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Employer|Applicant)$", ErrorMessage = "Role must be 'Employer' or 'Applicant'.")]
        public string Role { get; set; }
    }
}
