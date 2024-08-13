using Microsoft.EntityFrameworkCore;
using N5Challenge.Core.Entities;

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

            modelBuilder.Entity<Permission>().Property(p => p.PermissionDate)
                .HasColumnType("date");

            // Data Seeding
            SeedData(modelBuilder);
        }

        #region Data Seeding
        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissionType>().HasData(
                new PermissionType { Id = 1, Description = "Vacaciones" },
                new PermissionType { Id = 2, Description = "Permiso médico" },
                new PermissionType { Id = 3, Description = "Día personal" },
                new PermissionType { Id = 4, Description = "Licencia por estudios" },
                new PermissionType { Id = 5, Description = "Licencia por paternidad/maternidad" }
            );

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
                },
                new Permission
                {
                    Id = 3,
                    EmployeeName = "Alice",
                    EmployeeLastName = "Johnson",
                    PermissionTypeId = 3,
                    PermissionDate = DateTime.Now.AddDays(-1)
                },
                new Permission
                {
                    Id = 4,
                    EmployeeName = "Bob",
                    EmployeeLastName = "Brown",
                    PermissionTypeId = 4,
                    PermissionDate = DateTime.Now.AddDays(-7)
                },
                new Permission
                {
                    Id = 5,
                    EmployeeName = "Charlie",
                    EmployeeLastName = "Davis",
                    PermissionTypeId = 5,
                    PermissionDate = DateTime.Now.AddDays(-14)
                },
                new Permission
                {
                    Id = 6,
                    EmployeeName = "Diana",
                    EmployeeLastName = "Wilson",
                    PermissionTypeId = 5,
                    PermissionDate = DateTime.Now.AddDays(-3)
                },
                new Permission
                {
                    Id = 7,
                    EmployeeName = "Edward",
                    EmployeeLastName = "Taylor",
                    PermissionTypeId = 2,
                    PermissionDate = DateTime.Now.AddDays(-10)
                },
                new Permission
                {
                    Id = 8,
                    EmployeeName = "Fiona",
                    EmployeeLastName = "Thomas",
                    PermissionTypeId = 1,
                    PermissionDate = DateTime.Now.AddDays(5)
                },
                new Permission
                {
                    Id = 9,
                    EmployeeName = "George",
                    EmployeeLastName = "Anderson",
                    PermissionTypeId = 2,
                    PermissionDate = DateTime.Now.AddDays(2)
                },
                new Permission
                {
                    Id = 10,
                    EmployeeName = "Hannah",
                    EmployeeLastName = "Martinez",
                    PermissionTypeId = 3,
                    PermissionDate = DateTime.Now.AddDays(1)
                }

            );
        }
        #endregion
    }
}
