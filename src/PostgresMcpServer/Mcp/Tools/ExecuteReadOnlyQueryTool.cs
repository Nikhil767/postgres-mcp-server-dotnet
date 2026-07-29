using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModelContextProtocol.Server;
using PostgresMcpServer.Data;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostgresMcpServer.Mcp.Tools;

[McpServerToolType]
public static class ExecuteReadOnlyQueryTool
{

    [McpServerTool(Name = "execute_read_only_query"), Description("Executes a read-only SQL query (SELECT) against the database and returns the result as a JSON string.")]
    public static async Task<string> ExecuteQueryAsync(DynamicQueryExecutor queryExecutor, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return "Error: Query is empty.";
        }

		if (!QueryValidator.IsReadOnlySelect(query))
		{
			throw new InvalidOperationException("Security Violation: Only read-only SELECT queries are allowed.");
		}

		if (!query.Contains("LIMIT", StringComparison.OrdinalIgnoreCase))
		{
			query += " LIMIT 100";
		}

		try
        {
            var rows = await queryExecutor.ExecuteQueryAsync(query);
            return JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"Error executing query: {ex.Message}";
        }
    }
}
