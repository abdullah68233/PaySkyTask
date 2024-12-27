using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Exceptions
{
    public class CustomValidationException : ApplicationException
    {
        public CustomValidationException()
        {
        }

        public CustomValidationException(string message) : base(message)
        {
        }

    }
}
