using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Stringes;
using Stringes.Tokens;

namespace LexMyAss
{
    public static class Lexer
    {
        public static LexerRules<TokenType> Rules;

        static Lexer()
        {
            Rules = new LexerRules<TokenType>()
            {
                {"+", TokenType.Plus},
                {"-", TokenType.Minus},
                {"/", TokenType.Slash},
                {"*", TokenType.Asterisk},
                {"(", TokenType.LeftParen},
                {")", TokenType.RightParen},
                {"=", TokenType.Equals},
                {new Regex(@"-?\d+(\.\d+)?"), TokenType.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z\d_]*"), TokenType.Identifier}
            };
        }

        public static IEnumerable<Token<TokenType>> Lex(string inputString)
        {
            var reader = new StringeReader(inputString);
            Token<TokenType> token = null;
            while (!reader.EndOfStringe)
            {
                reader.SkipWhiteSpace();
                if (reader.EndOfStringe) yield break;

                if (!reader.EatToken(Rules, out token))
                {
                    var c = reader.PeekChare();
                    throw new FormatException(String.Concat("(Ln ", c.Line, ", Col ", c.Column, ") Invalid token '", c, "'"));
                }

                yield return token;
            }
        }
    }

    public enum TokenType
    {
        Plus,
        Equals,
        Minus,
        Slash,
        Asterisk,
        LeftParen,
        RightParen,
        Identifier,
        Number
    }
}