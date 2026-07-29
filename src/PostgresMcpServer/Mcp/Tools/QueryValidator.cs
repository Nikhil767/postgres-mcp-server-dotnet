using System.Text.RegularExpressions;

namespace PostgresMcpServer.Mcp.Tools
{
	public static class QueryValidator
	{
		// Matches queries starting with SELECT (ignoring leading whitespace/comments)
		private static readonly Regex SelectOnlyRegex = new(
			@"^\s*SELECT\b",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Matches forbidden SQL keywords that mutate database state
		private static readonly Regex ForbiddenKeywordsRegex = new(
			@"\b(INSERT|UPDATE|DELETE|DROP|ALTER|TRUNCATE|CREATE|GRANT|REVOKE|EXEC|EXECUTE)\b",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Validates that a SQL string is strictly a read-only SELECT statement.
		/// </summary>
		public static bool IsReadOnlySelect(string sqlQuery)
		{
			if (string.IsNullOrWhiteSpace(sqlQuery))
				return false;

			// Clean query of trailing semicolons or whitespace
			var trimmed = sqlQuery.Trim();

			// Ensure query starts with SELECT
			if (!SelectOnlyRegex.IsMatch(trimmed))
				return false;

			// Ensure query does NOT contain any state-mutating commands
			if (ForbiddenKeywordsRegex.IsMatch(trimmed))
				return false;

			return true;
		}
	}
}
