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
            Rules = new LexerRules<TokenType>
            {
                {"-", TokenType.Minus},
                {"+", TokenType.Plus},
                {"/", TokenType.Slash},
                {"*", TokenType.Asterisk},
                {"(", TokenType.LeftParen},
                {")", TokenType.RightParen},
                {"{", TokenType.LeftBrace},
                {"}", TokenType.RightBrace},
                {"=", TokenType.Assign},
                {"++", TokenType.Increment},
                {"--", TokenType.Decrement},
                {";", TokenType.Semicolon},
                {"%", TokenType.Modulo},
                {"^", TokenType.Caret},
                {new Regex(@"(int|uint|long|ulong|short|ushort|byte|sbyte|float|double|decimal|string)\b"), TokenType.Primitive, 3},
                {new Regex(@"-?\d+(\.\d+)?"), TokenType.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z\d_]*"), TokenType.Identifier, 2},
                {new Regex(@"""(([\r\n^""]|.|[^\\]"")*?[^\\])?""", RegexOptions.ExplicitCapture), TokenType.String}
            };

            Rules.AddUndefinedCaptureRule(TokenType.Misc, s => s.Trim());
        }

        public static IEnumerable<Token<TokenType>> Lex(string inputString)
        {
            var reader = new StringeReader(inputString);
            while (!reader.EndOfStringe)
            {
                reader.SkipWhiteSpace();
                yield return reader.ReadToken(Rules);
            }
        }
    }

    public enum TokenType
    {
        Plus,
        Increment,
        Decrement,
        Assign,
        Minus,
        Slash,
        Asterisk,
        Modulo,
        Caret,
        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,
        Semicolon,
        Identifier,
        Primitive,
        Number,
        String,
        Misc
    }
}