# Employee Management API

An ASP.NET Core 8 Web API for managing employees, built with EF Core (SQLite). The project demonstrates `async`/`await`, `CancellationToken` propagation, and a sequential vs. parallel performance comparison.

## Features

- Full CRUD for employees using asynchronous EF Core operations (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`)
- `CancellationToken` accepted by every endpoint and passed through the service layer to EF Core
- Benchmark endpoint that compares running two independent operations sequentially vs. in parallel (`Task.WhenAll`), measured with `Stopwatch`
- Swagger UI for exploring and testing the API

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8 with SQLite
- Swashbuckle (Swagger)

## Getting Started

Prerequisite: [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
dotnet restore
dotnet run
```

The database is created and seeded automatically on first run. Open Swagger at `/swagger` on the URL printed in the console.

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/employees` | Get all employees |
| GET | `/api/employees/{id}` | Get an employee by id |
| POST | `/api/employees` | Create an employee |
| PUT | `/api/employees/{id}` | Update an employee |
| DELETE | `/api/employees/{id}` | Delete an employee |
| GET | `/api/benchmark?rounds=5&latencyMs=1000` | Compare sequential vs. parallel execution |

Example request body for `POST` / `PUT`:

```json
{
  "name": "Mona Samir",
  "email": "mona@example.com",
  "salary": 9000,
  "departmentId": 1
}
```

## Benchmark

`GET /api/benchmark` runs two independent operations (counting employees and counting departments), each with its own `DbContext` created from `IDbContextFactory`, and returns the average time for both approaches.

| Parameter | Default | Description |
|-----------|---------|-------------|
| `rounds` | 5 | Number of measured rounds (1–100) |
| `latencyMs` | 1000 | Simulated I/O latency per operation (0–10000) |

## Project Structure

```
Controllers/   EmployeesController, BenchmarkController
Services/      IEmployeeService, EmployeeService, BenchmarkService
Data/          AppDbContext
Models/        Employee, Department, EmployeeRequest
Program.cs     Application setup and DI registration
```

## Using SQL Server

Replace `UseSqlite` with `UseSqlServer` in `Program.cs`, add the `Microsoft.EntityFrameworkCore.SqlServer` package, and update the connection string.
