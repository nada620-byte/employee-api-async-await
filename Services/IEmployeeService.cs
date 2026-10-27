using EmployeeApi.Models;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Employee> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken);
    Task<Employee?> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
