using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using PostgresMcpServer.Configuration;
using PostgresMcpServer.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Bind Database Options
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

// Configure DbContext with PostgreSQL Resiliency Retry logic
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var dbOptions = builder.Configuration
        .GetSection(DatabaseOptions.SectionName)
        .Get<DatabaseOptions>();
        
    var connectionString = dbOptions?.ConnectionString 
        ?? throw new InvalidOperationException("Database ConnectionString is not configured.");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});

// Register custom query executor
builder.Services.AddScoped<DynamicQueryExecutor>();

// Register MCP server with HTTP transport and scan for tools
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>{
		// Permits Postman per-request tool calls without rejecting strict _meta keys
		options.Stateless = true;
	})
    .WithToolsFromAssembly();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Returns instantly with HTTP 200 "Healthy" if the app isn't deadlocked.
app.MapHealthChecks("/api/alive", new HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("live")
});

// Map the MCP protocol handler endpoint
app.MapMcp("/mcp");

app.MapGet("/", () => "Postgres MCP Server is running.");

app.Run();
