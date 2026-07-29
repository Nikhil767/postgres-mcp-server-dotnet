using Xunit;
using FluentAssertions;
using PostgresMcpServer.Mcp.Tools;

namespace PostgresMcpServer.Tests;

public class QueryValidatorTests
{
	[Theory]
	[InlineData("SELECT * FROM users;")]
	[InlineData("select id, email from customers where active = true;")]
	public void ValidateQuery_ShouldAllow_ValidSelectQueries(string query)
	{
		bool isValid = QueryValidator.IsReadOnlySelect(query);
		isValid.Should().BeTrue();
	}

	[Theory]
	[InlineData("DROP TABLE users;")]
	[InlineData("DELETE FROM orders WHERE id = 1;")]
	[InlineData("UPDATE accounts SET balance = 0;")]
	[InlineData("INSERT INTO logs VALUES ('hack');")]
	public void ValidateQuery_ShouldBlock_MutatingQueries(string query)
	{
		bool isValid = QueryValidator.IsReadOnlySelect(query);
		isValid.Should().BeFalse();
	}
}
