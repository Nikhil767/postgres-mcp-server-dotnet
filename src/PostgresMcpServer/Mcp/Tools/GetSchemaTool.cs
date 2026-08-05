using ModelContextProtocol.Server;
using PostgresMcpServer.Data;
using System.ComponentModel;
using System.Text.Json;

namespace PostgresMcpServer.Mcp.Tools;

[McpServerToolType]
public static class GetSchemaTool
{
    [McpServerTool(Name = "get_database_schema"), Description("Inspects the PostgreSQL database schema and returns a compact JSON representation of all user tables and columns.")]
    public static async Task<string> GetSchemaAsync(DynamicQueryExecutor queryExecutor, CancellationToken cancellationToken = default)
    {
        const string query = """
            SELECT 
                t.table_schema AS schema, 
                t.table_name AS table, 
                c.column_name AS column, 
                c.data_type AS type, 
                c.is_nullable AS nullable
            FROM 
                information_schema.tables t
            JOIN 
                information_schema.columns c 
                ON t.table_schema = c.table_schema 
                AND t.table_name = c.table_name
            WHERE t.table_schema = 'public'
            ORDER BY 
                t.table_schema, 
                t.table_name, 
                c.ordinal_position;
            """;

        var rows = await queryExecutor.ExecuteQueryAsync(query, cancellationToken);
        
        var schemaMap = new Dictionary<string, Dictionary<string, List<object>>>();

        foreach (var row in rows)
        {
            var schemaName = row["schema"]?.ToString() ?? "public";
            var tableName = row["table"]?.ToString() ?? "";
            var columnName = row["column"]?.ToString() ?? "";
            var dataType = row["type"]?.ToString() ?? "";
            var isNullable = row["nullable"]?.ToString() ?? "YES";

            if (!schemaMap.TryGetValue(schemaName, out var tables))
            {
                tables = new Dictionary<string, List<object>>();
                schemaMap[schemaName] = tables;
            }

            if (!tables.TryGetValue(tableName, out var columns))
            {
                columns = new List<object>();
                tables[tableName] = columns;
            }

            columns.Add(new { Column = columnName, Type = dataType, Nullable = isNullable == "YES" });
        }

        return JsonSerializer.Serialize(schemaMap, new JsonSerializerOptions { WriteIndented = true });
    }
}
