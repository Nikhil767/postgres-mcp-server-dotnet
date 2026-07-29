# System Architecture: PostgreSQL MCP Server (.NET 10)

## Overview
An enterprise-grade Model Context Protocol (MCP) server built in C# .NET 10 using Npgsql / Entity Framework Core. It exposes PostgreSQL database schemas (Supabase) and safe read-only SQL querying capabilities as MCP Tools to AI Agents.

## Directory Structure
├── src/
│   └── PostgresMcpServer/
│       ├── Configuration/
│       │   └── DatabaseOptions.cs
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── DynamicQueryExecutor.cs
│       ├── Mcp/
│       │   ├── Tools/
│       │   │   ├── GetSchemaTool.cs
│       │   │   └── ExecuteReadOnlyQueryTool.cs
│       │   └── McpServerExtensions.cs
│       ├── Program.cs
│       └── appsettings.json
├── .agents/
│   └── rules/
│       └── mcp-dotnet-rules.md
└── PostgresMcpServer.sln

## MCP Tools Exposed
1. `get_database_schema`:
   - Inspects PostgreSQL `information_schema` and returns compact JSON tables/columns.
2. `execute_read_only_query`:
   - Safely executes SELECT queries against Supabase PostgreSQL and returns JSON records.