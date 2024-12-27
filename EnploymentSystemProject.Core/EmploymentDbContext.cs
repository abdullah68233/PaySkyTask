using EmploymentSystemProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EnploymentSystemProject.Core
{
    public class EmploymentDbContext : DbContext
    {
        public EmploymentDbContext(DbContextOptions<EmploymentDbContext> options) : base(options) { }

        public DbSet<Vacancy> Vacancies { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ArchivedVacancy> ArchivedVacancies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Vacancy Table
            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Title).IsRequired().HasMaxLength(200);
                entity.Property(v => v.Description).HasMaxLength(1000);
                entity.Property(v => v.MaxApplications).IsRequired();
                entity.Property(v => v.ExpiryDate).IsRequired();
                entity.Property(v => v.IsActive).IsRequired();

            });

            // Application Table
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.ApplicationDate).IsRequired();

                // Foreign keys
                entity.HasOne(a => a.Vacancy)
                      .WithMany(v => v.Applications)
                      .HasForeignKey(a => a.VacancyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Applicant)
                      .WithMany()
                      .HasForeignKey(a => a.ApplicantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // User Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });

            // ArchivedVacancy Table
            modelBuilder.Entity<ArchivedVacancy>(entity =>
            {
                entity.HasKey(av => av.Id);
                entity.Property(av => av.Title).IsRequired().HasMaxLength(200);
                entity.Property(av => av.Description).HasMaxLength(1000);
                entity.Property(av => av.ArchivedDate).IsRequired();
            });
        }
    }
}
