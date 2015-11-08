using System;

namespace Stringes.RegularExpressions
{
	/// <summary>
	/// Represents an immutable regular expression.
	/// </summary>
	public sealed class Regex
	{
		private static readonly Lexer<RE> RegexLexer;

		static Regex()
		{
			RegexLexer = new Lexer<RE>
			{
				{
					reader =>
					{
						if (!reader.Eat('\\')) return false;
						reader.ReadChare();
						return true;
					},
					RE.Escape
				},
				{ reader => reader.EatWhile(char.IsWhiteSpace), RE.Whitespace },
				{ "(", RE.LeftParen },
				{ "}", RE.RightParen },
				{ "[", RE.LeftSquare },
				{ "]", RE.RightSquare },
				{ "{", RE.LeftCurly },
				{ "}", RE.RightCurly },
				{ "+", RE.Plus }
			};
			RegexLexer.AddUndefinedCaptureRule(RE.Text, s => s);
		}

		private readonly string _pattern;
		private readonly RegexOptions _options;

		/// <summary>
		/// Gets the pattern string of the current Regexe object.
		/// </summary>
		public string Pattern => _pattern;

		/// <summary>
		/// Gets the options applied to the current Regexe object.
		/// </summary>
		public RegexOptions Options => _options;

		/// <summary>
		/// Initializes a new Regexe object with the specified pattern.
		/// </summary>
		/// <param name="pattern">The pattern to create a Regexe from.</param>
		public Regex(string pattern)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));
			_pattern = pattern;
			_options = RegexOptions.None;
		}

		/// <summary>
		/// Initializes a new Regexe object with the specified pattern and options.
		/// </summary>
		/// <param name="pattern">The pattern to create a Regexe from.</param>
		/// <param name="options">The options to apply to the instance.</param>
		public Regex(string pattern, RegexOptions options)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));
			_pattern = pattern;
			_options = options;
		}
	}
}