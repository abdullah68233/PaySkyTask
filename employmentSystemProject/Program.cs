using EmploymentSystemProject.DSL;
using EmploymentSystemProject.Repo;
using EnploymentSystemProject.Core;
using Microsoft.EntityFrameworkCore;
using EmploymentSystemProject.Helpers;
using Microsoft.OpenApi.Models;

namespace employmentSystemProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add Controllers
            services.AddControllers();

            // Configure DbContext with Connection String
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EmploymentDbContext>(options => options.UseSqlServer(connectionString));

            // Add Swagger with Security
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Employment System API",
                    Version = "v1",
                    Description = "API Documentation for Employment System Project"
                });

                // Add Bearer Token Security
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add Memory Cache
            services.AddMemoryCache();

            // Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Dependency Injection for Repositories
            services.AddScoped<EmployerRepo>();
            services.AddScoped<ApplicantRepo>();
            services.AddScoped<LoginRepo>();

            // Dependency Injection for DSLs
            services.AddScoped<EmployerDSL>();
            services.AddScoped<ApplicantDSL>();
            services.AddScoped<LoginDSL>();

            // Add Token Manager
            services.AddScoped<TokenManager>();

            // Add Error Handling Middleware
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            // Enable Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employment System API v1");
                    options.RoutePrefix = string.Empty; // Set Swagger UI as the root
                });
            }

            // Enable CORS
            app.UseCors("AllowAll");

            // Add HTTPS Redirection
            app.UseHttpsRedirection();

            app.MapFallback(context =>
            {
                context.Response.Redirect("/index.html");
                return Task.CompletedTask;
            });

            // Use Error Handling Middleware
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // Add Authorization Middleware
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();
        }
    }
}
