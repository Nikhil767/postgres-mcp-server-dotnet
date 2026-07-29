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

## 🛠️ Local Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* A PostgreSQL / Supabase connection string

### Setup
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/Nikhil767/postgres-mcp-server-dotnet.git](https://github.com/Nikhil767/postgres-mcp-server-dotnet.git)
   cd postgres-mcp-server-dotnet
   
## ⚙️ Configuration & Setup

## 🤝 Contributing

Contributions are welcome. Fork the repo, create a feature branch, and submit a PR with a clear description of your change.

```bash
git checkout -b feature/your-feature-name
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2025 Nikhil

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```