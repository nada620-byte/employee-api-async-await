using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering" },
            new Department { Id = 2, Name = "HR" },
            new Department { Id = 3, Name = "Finance" });

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Name = "Ahmed Ali", Email = "ahmed@example.com", Salary = 12000, DepartmentId = 1 },
            new Employee { Id = 2, Name = "Sara Hassan", Email = "sara@example.com", Salary = 10000, DepartmentId = 2 },
            new Employee { Id = 3, Name = "Omar Khaled", Email = "omar@example.com", Salary = 11000, DepartmentId = 3 });
    }
}
