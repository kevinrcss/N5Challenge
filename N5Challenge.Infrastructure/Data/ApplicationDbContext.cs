using Microsoft.EntityFrameworkCore;
using N5Challenge.Core.Entities;
using System.Security;

namespace N5Challenge.Infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.PermissionType)
                .WithMany(pt => pt.Permissions)
                .HasForeignKey(p => p.PermissionTypeId);
        }

        #region Data Seeding
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed PermissionTypes
            modelBuilder.Entity<PermissionType>().HasData(
                new PermissionType { Id = 1, Description = "Vacaciones" },
                new PermissionType { Id = 2, Description = "Permiso médico" },
                new PermissionType { Id = 3, Description = "Día personal" }
            );

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    Id = 1,
                    EmployeeName = "John",
                    EmployeeLastName = "Doe",
                    PermissionTypeId = 1,
                    PermissionDate = DateTime.Now.AddDays(-5)
                },
                new Permission
                {
                    Id = 2,
                    EmployeeName = "Jane",
                    EmployeeLastName = "Smith",
                    PermissionTypeId = 2,
                    PermissionDate = DateTime.Now.AddDays(-2)
                }
            );
        }
        #endregion
    }
}
