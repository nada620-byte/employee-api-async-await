using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context) => _context = context;

    // Task<List<T>> - ToListAsync بياخد الـ CancellationToken
    public Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        // مفيش معالجة بعد الاستعلام، فنرجع الـ Task مباشرة من غير async/await زيادة
        return _context.Employees
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            Name = request.Name,
            Email = request.Email,
            Salary = request.Salary,
            DepartmentId = request.DepartmentId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
            return null;

        employee.Name = request.Name;
        employee.Email = request.Email;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;

        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
