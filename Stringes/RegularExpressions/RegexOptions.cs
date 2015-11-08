using System;

namespace Stringes.RegularExpressions
{
	/// <summary>
	/// Defines options that customize the behavior of regex searches.
	/// </summary>
	[Flags]
	public enum RegexOptions : byte
	{
		/// <summary>
		/// Nothing... Nothing at all.
		/// </summary>
		None =				0x00,
		/// <summary>
		/// Disregard character case when searching strings.
		/// </summary>
		IgnoreCase =		0x01,
		/// <summary>
		/// Disregard whitespace in the search pattern.
		/// </summary>
		IgnoreWhitespace =	0x02,
		/// <summary>
		/// Split the input string by line so that the ^ and $ anchors
		/// match at line boundaries instead of at the start and end of the string.
		/// </summary>
		Multiline =			0x04,
	}
}