using EmployeeApi.Data;
using EmployeeApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite علشان المشروع يشتغل على أي جهاز من غير تثبيت SQL Server.
// لو عايز SQL Server: غيّر UseSqlite إلى UseSqlServer وضيف package Microsoft.EntityFrameworkCore.SqlServer.
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=employees.db";

// Factory مطلوب للقياس المتوازي (DbContext مستقل لكل عملية)
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(connectionString));

// AppDbContext عادي (Scoped) للـ Controllers والـ Services، بيتعمل من نفس الـ Factory
builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<BenchmarkService>();

var app = builder.Build();

// إنشاء قاعدة البيانات مع بيانات تجريبية أول مرة
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// لو العميل قفل الاتصال، الـ CancellationToken بيرمي OperationCanceledException.
// ده سلوك طبيعي مش خطأ، فبنرجّع 499 بدل ما يظهر كـ 500 في الـ logs.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        context.Response.StatusCode = 499; // Client Closed Request
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
