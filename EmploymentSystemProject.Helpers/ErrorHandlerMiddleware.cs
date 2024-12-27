using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EmploymentSystemProject.Entities;
using EmploymentSystemProject.Exceptions;
using Microsoft.AspNetCore.Http;

namespace EmploymentSystemProject.Helpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            File_Logger.WriteToLogFile(ActionType.Exception, exception.ToString());

            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                CustomValidationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var message = exception switch
            {
                UnauthorizedException => "Not Authorized",
                CustomValidationException customEx => customEx.Message,
                KeyNotFoundException => "Resource not found",
                _ => "An unexpected error occurred. Please try again later."
            };

            response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new ResponseDTO<string>(false, message), _jsonOptions);
            await response.WriteAsync(result);
        }
    }
}
