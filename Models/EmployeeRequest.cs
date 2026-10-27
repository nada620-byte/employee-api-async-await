using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Models;

// DTO بيستخدم في Create و Update (علشان العميل ما يبعتش Id بنفسه)
public class EmployeeRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Range(0, 10_000_000)]
    public decimal Salary { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }
}
