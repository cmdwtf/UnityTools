using System.Text.RegularExpressions;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Represents a wildcard running on the
	/// <see cref="System.Text.RegularExpressions"/> engine.
	/// </summary>
	public class Wildcard : Regex
	{
		/// <summary>
		/// Initializes a wildcard with the given search pattern.
		/// </summary>
		/// <param name="pattern">The wildcard pattern to match.</param>
		public Wildcard(string pattern)
			: base(WildcardToRegex(pattern))
		{
		}

		/// <summary>
		/// Initializes a wildcard with the given search pattern and options.
		/// </summary>
		/// <param name="pattern">The wildcard pattern to match.</param>
		/// <param name="options">A combination of one or errors
		/// <see cref="System.Text.RegexOptions"/>.</param>
		public Wildcard(string pattern, RegexOptions options)
			: base(WildcardToRegex(pattern), options)
		{
		}

		/// <summary>
		/// Converts a wildcard to a regex.
		/// </summary>
		/// <param name="pattern">The wildcard pattern to convert.</param>
		/// <returns>A regex equivalent of the given wildcard.</returns>
		public static string WildcardToRegex(string pattern)
			=> $"^{Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".")}$";
	}
}