using Microsoft.EntityFrameworkCore;
using System.Data;

namespace PostgresMcpServer.Data;

public class DynamicQueryExecutor
{
    private readonly ApplicationDbContext _dbContext;

    public DynamicQueryExecutor(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Dictionary<string, object?>>> ExecuteQueryAsync(string sql)
    {
        var results = new List<Dictionary<string, object?>>();
        var connection = _dbContext.Database.GetDbConnection();
        
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed)
        {
            await connection.OpenAsync();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }
                results.Add(row);
            }
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }

        return results;
    }
}
