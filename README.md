# PostgreSQL MCP Server (.NET 10)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Protocol](https://img.shields.io/badge/MCP-2025--11--25-blue)](https://modelcontextprotocol.io/)
[![Database](https://img.shields.io/badge/PostgreSQL-Supabase-4169E1?logo=postgresql)](https://supabase.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

An enterprise-ready **Model Context Protocol (MCP)** server built with C# and ASP.NET Core Minimal APIs. It enables AI Assistants (Cursor, Claude Desktop, custom agents) to inspect database structures and execute safe, read-only SQL queries against PostgreSQL instances like **Supabase**.

---

## 🌟 Key Features

* **Safe SQL Execution Engine**: Rigid regex & AST-based query validation that blocks mutating SQL statements (`INSERT`, `UPDATE`, `DELETE`, `DROP`, `ALTER`, etc.).
* **PostgreSQL / Supabase Ready**: Built on top of `Npgsql.EntityFrameworkCore.PostgreSQL` for optimized metadata extraction and pooling.
* **JSON-RPC / HTTP-SSE Support**: Compliant with the official C# `ModelContextProtocol.AspNetCore` SDK.
* **xUnit Test Suite**: Includes automated unit tests verifying security guardrails before execution.
* **Docker Ready**: Multi-stage `Dockerfile` optimized for zero-dependency container deployment (Render, Azure, AWS).

---

## 🛠️ Tech Stack

* **Framework**: .NET 10 (ASP.NET Core Minimal API)
* **Protocol**: `ModelContextProtocol.AspNetCore` (v2.0.0-rc.2)
* **Database**: Entity Framework Core 10 & Npgsql PostgreSQL Provider
* **Testing**: xUnit & FluentAssertions

---

## 🚀 Available MCP Tools

| Tool Name | Parameters | Description |
| :--- | :--- | :--- |
| `get_database_schema` | `tableName` *(optional string)* | Inspects `information_schema` and returns table columns, primary keys, and data types. |
| `execute_read_only_query` | `query` *(required string)* | Validates and executes read-only `SELECT` queries, returning formatted JSON rows. |

---

## 🔒 Security & Guardrails

The server enforces strict read-only execution at the application level:
* Rejects any query not explicitly starting with `SELECT`.
* Rejects multi-statement or stacked SQL queries containing semicolon chaining (`SELECT * FROM a; DROP TABLE b;`).
* Rejects data-definition (DDL) and data-manipulation (DML) statements.

---

## 💻 Connecting to AI Clients

### Claude Desktop Configuration
Add the following to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "postgres-mcp": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/postgres-mcp-server-dotnet/src/PostgresMcpServer"
      ]
    }
  }
}
```
---

## 🛠️ Local Setup & Development

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* A PostgreSQL / Supabase connection string

### Setup
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/Nikhil767/postgres-mcp-server-dotnet.git](https://github.com/Nikhil767/postgres-mcp-server-dotnet.git)
   cd postgres-mcp-server-dotnet
	```
2. **Configure Environment:**
Set your connection string in src/PostgresMcpServer/appsettings.json or pass it via environment variable:
```
export ConnectionStrings__DefaultConnection="Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;Port=5432;"
```
3. **Run Unit Tests:**
```
dotnet test
```
4. **Start the MCP Server:**
```
dotnet run --project src/PostgresMcpServer
```
The endpoint listens at http://localhost:8080/mcp.

---

## 📮 API & Postman Testing
A pre-configured Postman/Bruno collection is available in the /docs folder to test tool calls directly via HTTP JSON-RPC payloads.

---

## 🤝 Contributing

Contributions are welcome. Fork the repo, create a feature branch, and submit a PR with a clear description of your change.

```bash
git checkout -b feature/your-feature-name
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

---

## 📄 License

This project is licensed under the [**MIT License**](https://github.com/Nikhil767/postgres-mcp-server-dotnet/blob/main/LICENSE).

Thank you for reading to the end! 