# Employee Management API — Async/Await & Performance

ASP.NET Core 8 Web API + EF Core (SQLite).

## التشغيل

```bash
dotnet restore
dotnet run
```

افتحي Swagger على الرابط اللي بيظهر في الـ console (مثلاً `http://localhost:5000/swagger`).

## الـ Endpoints

| Method | Route | الوصف |
|---|---|---|
| GET | /api/employees | GetAll |
| GET | /api/employees/{id} | Get |
| POST | /api/employees | Create |
| PUT | /api/employees/{id} | Update |
| DELETE | /api/employees/{id} | Delete |
| GET | /api/benchmark?rounds=5&latencyMs=1000 | مقارنة Sequential vs Parallel |

## مثال Body لـ POST / PUT

```json
{ "name": "Mona Samir", "email": "mona@example.com", "salary": 9000, "departmentId": 1 }
```

## ملاحظات

- كل methods في الـ Controller والـ Service بتاخد `CancellationToken` وبتمرره لـ EF Core.
- للتحويل إلى SQL Server: غيّري `UseSqlite` إلى `UseSqlServer` وضيفي package `Microsoft.EntityFrameworkCore.SqlServer` وعدّلي الـ connection string.
- التقرير في `REPORT.md` (املي جدول الأرقام من نتيجة جهازك).
