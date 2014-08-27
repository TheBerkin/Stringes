using System.Collections.Generic;
using System.Text.RegularExpressions;

using Stringes;
using Stringes.Tokens;

namespace LexMyAss
{
    public static class Lexer
    {
        private static readonly Regex EscapeRegex = new Regex(@"\\([^u\s]|u[0-9a-f]{4})");
        private static readonly Regex RegexRegex = new Regex(@"/(.*?[^\\])?/i?");
        private static readonly Regex CommentRegex = new Regex(@"``(([\r\n]|.)*?[^\\])?``");

        private static readonly LexerRules<TokenType> Rules;

        private static Stringe TruncatePadding(Stringe input)
        {
            var ls = input.LeftPadded ? input.TrimStart() : input;
            return ls.RightPadded ? ls.TrimEnd() : ls;
        }

        static Lexer()
        {
            Rules = new LexerRules<TokenType>
            {
                {EscapeRegex, TokenType.EscapeSequence},
                {RegexRegex, TokenType.Regex},
                {"[", TokenType.LeftSquare}, {"]", TokenType.RightSquare},
                {"{", TokenType.LeftCurly}, {"}", TokenType.RightCurly},
                {"(", TokenType.LeftParen}, {")", TokenType.RightParen},
                {"<", TokenType.LeftTriangle}, {">", TokenType.RightTriangle},
                {"|", TokenType.Pipe},
                {";", TokenType.Semicolon},
                {":", TokenType.Colon},
                {"@", TokenType.At},
                {"?", TokenType.Question},
                {"::", TokenType.DoubleColon},
                {"?!", TokenType.Without},
                {"-", TokenType.Hyphen},
                {"!", TokenType.Exclamation},
                {"$", TokenType.Dollar},
                {CommentRegex, TokenType.Comment}
            };
            Rules.AddUndefinedCaptureRule(TokenType.Text, TruncatePadding);
            Rules.AddEndToken(TokenType.EOF);
        }

        public static IEnumerable<Token<TokenType>> Lex(string input)
        {
            var reader = new StringeReader(input);
            while (!reader.EndOfStringe)
            {
                yield return reader.ReadToken(Rules);
            }
        }
    }

    public enum TokenType
    {
        /// <summary>
        /// Regular text with no special function.
        /// </summary>
        Text,
        /// <summary>
        /// An alphanumeric identifier string with no spaces, but can contain underscores.
        /// <para>
        /// Used by: Tags (names), Queries (list IDs, subtypes, classes)
        /// </para>
        /// </summary>
        Identifier,
        /// <summary>
        /// A format string used to output a reserved or random character.
        /// Used by: Plaintext, arguments
        /// </summary>
        EscapeSequence,
        /// <summary>
        /// [
        /// <para>
        /// Used by: Tags (opening)
        /// </para>
        /// </summary>
        LeftSquare,
        /// <summary>
        /// ]
        /// <para>
        /// Used by: Tags (closure)
        /// </para>
        /// </summary>
        RightSquare,
        /// <summary>
        /// {
        /// <para>
        /// Used by: Blocks (opening)
        /// </para>
        /// </summary>
        LeftCurly,
        /// <summary>
        /// }
        /// <para>
        /// Used by: Blocks (closure)
        /// </para>
        /// </summary>
        RightCurly,
        /// <summary>
        /// &lt;
        /// <para>
        /// Used by: Queries (opening)
        /// </para>
        /// </summary>
        LeftTriangle,
        /// <summary>
        /// &gt;
        /// <para>
        /// Used by: Queries (closure)
        /// </para>
        /// </summary>
        RightTriangle,
        /// <summary>
        /// (
        /// <para>
        /// Used by: Arithmetic (opening)
        /// </para>
        /// </summary>
        LeftParen,
        /// <summary>
        /// )
        /// <para>
        /// Used by: Arithmetic (closure)
        /// </para>
        /// </summary>
        RightParen,
        /// <summary>
        /// |
        /// <para>
        /// Used by: Blocks (item separator)
        /// </para>
        /// </summary>
        Pipe,
        /// <summary>
        /// :
        /// <para>
        /// Used by: Tags (follows name)
        /// </para>
        /// </summary>
        Colon,
        /// <summary>
        /// ;
        /// <para>
        /// Used by: Tags (argument separator)
        /// </para>
        /// </summary>
        Semicolon,
        /// <summary>
        /// ::
        /// <para>
        /// Used by: Queries (carrier operator)
        /// </para>
        /// </summary>
        DoubleColon,
        /// <summary>
        /// @
        /// <para>
        /// Used by: Tags (constant arg notation), Arithmetic (statement modifier)
        /// </para>
        /// </summary>
        At,
        /// <summary>
        /// ?
        /// <para>
        /// Used by: Tags (metapatterns), Queries (whitelist regex)
        /// </para>
        /// </summary>
        Question,
        /// <summary>
        /// /
        /// <para>
        /// Used by: Queries (regex filters)
        /// </para>
        /// </summary>
        ForwardSlash,
        /// <summary>
        /// !
        /// <para>
        /// Used by: Queries ('not' class constraint modifier)
        /// </para>
        /// </summary>
        Exclamation,
        /// <summary>
        /// $
        /// <para>
        /// Used by: Queries ('only' modifier)
        /// </para>
        /// </summary>
        Dollar,
        /// <summary>
        /// -
        /// <para>
        /// Used by: Queries (class constraint)
        /// </para>
        /// </summary>
        Hyphen,
        /// <summary>
        /// ,
        /// <para>
        /// Used by: Queries (subtype prefix)
        /// </para>
        /// </summary>
        Comma,
        /// <summary>
        /// ?!
        /// <para>
        /// Used by: Queries (blacklist regex)
        /// </para>
        /// </summary>
        Without,
        /// <summary>
        /// Javascript-style regular expression.
        /// <para>
        /// Used by: Queries (blacklist/whitelist)
        /// </para>
        /// </summary>
        Regex,
        /// <summary>
        /// ``
        /// </summary>
        Comment,
        /// <summary>
        /// End of file.
        /// </summary>
        EOF
    }
}