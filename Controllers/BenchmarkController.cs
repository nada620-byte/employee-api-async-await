using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BenchmarkController : ControllerBase
{
    private readonly BenchmarkService _benchmark;

    public BenchmarkController(BenchmarkService benchmark) => _benchmark = benchmark;

    // GET api/benchmark?rounds=5&latencyMs=1000   -> عمليات فيها انتظار I/O (التوازي هيفرق)
    // GET api/benchmark?rounds=5&latencyMs=0      -> استعلامات SQLite الحقيقية بس (التوازي غالباً مش هيفرق)
    [HttpGet]
    public async Task<ActionResult<BenchmarkResult>> Run(
        [FromQuery] int rounds = 5,
        [FromQuery] int latencyMs = 1000,
        CancellationToken cancellationToken = default)
    {
        if (rounds is < 1 or > 100)
            return BadRequest("rounds لازم تكون بين 1 و 100.");
        if (latencyMs is < 0 or > 10_000)
            return BadRequest("latencyMs لازم تكون بين 0 و 10000.");

        var result = await _benchmark.RunAsync(rounds, latencyMs, cancellationToken);
        return Ok(result);
    }
}
