using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace EmploymentSystemProject.Helpers
{
    public class ConfigurationHelper
    {
        private readonly IConfiguration _configuration;
        public string ConnectionString { get; }
        public string LogFilePath { get; }
        public ConfigurationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = GetKey<string>("ConnectionString", true);
            LogFilePath = GetKey<string>("LogFilePath") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        private T GetKey<T>(string key, bool isConnectionString = false)
        {
            if (isConnectionString)
            {
                return (T)Convert.ChangeType(_configuration.GetConnectionString(key), typeof(T));
            }

            return (T)Convert.ChangeType(_configuration.GetSection(key).Value, typeof(T));
        }
    }
}
