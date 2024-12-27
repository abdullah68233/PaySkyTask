using EmploymentSystemProject.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Helpers
{
    public class RequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            File_Logger.WriteToLogFile(ActionType.Information, "start request=============================");
            try
            {
                string requestBody;
                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                File_Logger.WriteToLogFile(ActionType.Information, $"request method: {context.Request.Method}, request body: {requestBody}");
            }
            finally
            {
                context.Request.Body.Position = 0;
            }


            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;
                    await _next(context);
                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();
                    File_Logger.WriteToLogFile(ActionType.Information, $"response body: {responseBody}");
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
            File_Logger.WriteToLogFile(ActionType.Information, "end request=============================");
        }
    }
}
