using System.Diagnostics;
using EmployeeApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services;

public record BenchmarkResult(
    int Rounds,
    int SimulatedLatencyMs,
    double SequentialAvgMs,
    double ParallelAvgMs,
    double SequentialTotalMs,
    double ParallelTotalMs,
    double SpeedUp,
    string Conclusion);

public class BenchmarkService
{
    // كل عملية بتاخد DbContext خاص بيها من الـ Factory،
    // لأن EF Core ما بيدعمش عمليات متوازية على نفس الـ DbContext.
    private readonly IDbContextFactory<AppDbContext> _factory;

    public BenchmarkService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    // عملية مستقلة 1: عدّ الموظفين + تأخير I/O مُحاكى
    private async Task<int> CountEmployeesAsync(int latencyMs, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var count = await db.Employees.CountAsync(ct);
        if (latencyMs > 0)
            await Task.Delay(latencyMs, ct);
        return count;
    }

    // عملية مستقلة 2: عدّ الأقسام + تأخير I/O مُحاكى
    private async Task<int> CountDepartmentsAsync(int latencyMs, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var count = await db.Departments.CountAsync(ct);
        if (latencyMs > 0)
            await Task.Delay(latencyMs, ct);
        return count;
    }

    private async Task RunSequentialAsync(int latencyMs, CancellationToken ct)
    {
        await CountEmployeesAsync(latencyMs, ct);
        await CountDepartmentsAsync(latencyMs, ct);
    }

    private async Task RunParallelAsync(int latencyMs, CancellationToken ct)
    {
        // بنبدأ العمليتين الأول من غير await، وبعدين نستنى الاتنين مع بعض
        var employeesTask = CountEmployeesAsync(latencyMs, ct);
        var departmentsTask = CountDepartmentsAsync(latencyMs, ct);
        await Task.WhenAll(employeesTask, departmentsTask);
    }

    public async Task<BenchmarkResult> RunAsync(int rounds, int latencyMs, CancellationToken ct)
    {
        // Warm-up: بيحمّي الـ JIT والـ connection علشان أول تشغيل ما يبوّظش القياس
        await RunSequentialAsync(0, ct);
        await RunParallelAsync(0, ct);

        var seqTimes = new List<double>();
        var parTimes = new List<double>();

        for (var i = 0; i < rounds; i++)
        {
            var sw = Stopwatch.StartNew();
            await RunSequentialAsync(latencyMs, ct);
            sw.Stop();
            seqTimes.Add(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            await RunParallelAsync(latencyMs, ct);
            sw.Stop();
            parTimes.Add(sw.Elapsed.TotalMilliseconds);
        }

        var seqAvg = seqTimes.Average();
        var parAvg = parTimes.Average();
        var speedUp = parAvg > 0 ? seqAvg / parAvg : 0;

        var conclusion = speedUp > 1.2
            ? "التوازي حسّن الأداء فعلاً لأن العمليتين مستقلتين وبيستنوا I/O."
            : "التوازي ما حسّنش الأداء بشكل ملحوظ لأن العمليات سريعة جداً (الـ overhead بيساوي المكسب).";

        return new BenchmarkResult(
            rounds,
            latencyMs,
            Math.Round(seqAvg, 2),
            Math.Round(parAvg, 2),
            Math.Round(seqTimes.Sum(), 2),
            Math.Round(parTimes.Sum(), 2),
            Math.Round(speedUp, 2),
            conclusion);
    }
}
