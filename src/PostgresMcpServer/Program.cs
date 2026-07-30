using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using PostgresMcpServer.Configuration;
using PostgresMcpServer.Data;

//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

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
builder.Services.AddMcpServer()//.WithHttpTransport().WithToolsFromAssembly();
.WithHttpTransport(options =>
{
	// Permits Postman per-request tool calls without rejecting strict _meta keys
	options.Stateless = true;
}).WithToolsFromAssembly();

builder.Services.AddHealthChecks();

builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("mcpPolicy", opt =>
	{
		opt.PermitLimit = 30; // 30 requests per minute
		opt.Window = TimeSpan.FromMinutes(1);
	});
});

var app = builder.Build();

app.Use(async (context, next) =>
{
	if (context.Request.Path.StartsWithSegments("/mcp"))
	{
		var expectedKey = app.Configuration["McpApiKey"];
		// Check X-Api-Key OR X-MCP-API-KEY OR Authorization: Bearer <key>
		bool isAuthorized = (context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) && apiKey == expectedKey) ||
		(context.Request.Headers.TryGetValue("X-MCP-API-KEY", out var mcpApiKey) && mcpApiKey == expectedKey) ||
		(context.Request.Headers.TryGetValue("Authorization", out var authHeader) && authHeader.ToString() == $"Bearer {expectedKey}");
		if (!isAuthorized)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("Unauthorized: Invalid API Key");
			return;
		}
	}
	await next();
});

app.MapHealthChecks("/api/alive", new HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("live")
});

// Map the MCP protocol handler endpoint
app.MapMcp("/mcp");

app.MapGet("/", () => "Postgres MCP Server is running.");

app.Run();
