# Implementation Tasks

- [x] 1. Create a new .NET 10 Minimal API project in `src/PostgresMcpServer` using `dotnet new web -o src/PostgresMcpServer` and create solution `PostgresMcpServer.sln`.
- [x] 2. Add required NuGet packages: `ModelContextProtocol.AspNetCore` and `Npgsql.EntityFrameworkCore.PostgreSQL`.
- [x] 3. Create `appsettings.json` configured for Supabase PostgreSQL connection string.
- [x] 4. Create `ApplicationDbContext.cs` and `DatabaseOptions.cs`.
- [x] 5. Build `GetSchemaTool` to inspect PostgreSQL `information_schema` and return compact JSON schemas.
- [x] 6. Build `ExecuteReadOnlyQueryTool` with validation blocking mutating SQL queries (`INSERT`, `UPDATE`, `DELETE`, `DROP`, `ALTER`, etc.).
- [x] 7. Register MCP services and endpoint mapping (`app.MapMcp("/mcp")`) in `Program.cs`.
- [x] 8. Verify `dotnet build` succeeds with 0 errors.