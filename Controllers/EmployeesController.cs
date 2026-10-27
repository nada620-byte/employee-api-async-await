using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    // GET api/employees
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _service.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    // GET api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> Get(int id, CancellationToken cancellationToken)
    {
        var employee = await _service.GetByIdAsync(id, cancellationToken);
        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // POST api/employees
    [HttpPost]
    public async Task<ActionResult<Employee>> Create(
        [FromBody] EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT api/employees/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Employee>> Update(
        int id,
        [FromBody] EmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    // DELETE api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
