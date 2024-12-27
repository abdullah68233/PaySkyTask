using EmploymentSystemProject.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Helpers
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == null || !(role == "Employer" || role == "Applicant"))
                    throw new UnauthorizedException();
            }

            await _next(context);
        }
    }

}
