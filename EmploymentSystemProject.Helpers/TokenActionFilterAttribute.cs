using EmploymentSystemProject.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Helpers
{
    public class TokenActionFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenManager = (TokenManager)context.HttpContext.RequestServices.GetService(typeof(TokenManager));

            if (tokenManager == null)
            {
                return;
            }
            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
            try
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                tokenManager.Validate(token);
            }
            catch (Exception)
            {
                throw new UnauthorizedException();
            }
        }
    }
}
