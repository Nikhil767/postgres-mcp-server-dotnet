---
description: Rules for building the Enterprise .NET 10 MCP Server
activation: AlwaysOn
---

# Code Generation & Security Rules for .NET MCP Server

## Technical Stack
- Framework: C# .NET 10 (Minimal APIs / ASP.NET Core)
- Protocol: Official Model Context Protocol C# SDK (`ModelContextProtocol.AspNetCore`)
- ORM: Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.PostgreSQL` and `Microsoft.EntityFrameworkCore.SqlServer`)
- Target DB: Supabase PostgreSQL (Primary) / MSSQL (Secondary)

## Security & Architecture Constraints
1. **Read-Only Enforcer:** The database inspection & query tool MUST ONLY execute read-only queries (`SELECT`). Block any `INSERT`, `UPDATE`, `DELETE`, `DROP`, `ALTER`, `TRUNCATE`, or `EXEC` statements before sending to the database.
2. **Parameterized Queries:** Never concatenate raw strings into SQL. Use EF Core `FromSqlRaw` with explicit parameter bindings or LINQ expressions.
3. **Token Efficiency:** When returning schema context to the LLM, strip auto-generated metadata, system schemas (`pg_catalog`, `information_schema`), and return compact JSON.
4. **Resiliency:** Use `EnableRetryOnFailure()` in EF Core connection options to handle cloud connection blinks (Supabase/MonsterASP).