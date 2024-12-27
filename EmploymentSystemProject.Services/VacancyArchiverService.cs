using EmploymentSystemProject.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Services
{
    public class VacancyArchiverService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public VacancyArchiverService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ArchiveExpiredVacancies, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private void ArchiveExpiredVacancies(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var employerRepo = scope.ServiceProvider.GetRequiredService<EmployerRepo>();
                employerRepo.ArchiveExpiredVacanciesAsync().Wait();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }

}
