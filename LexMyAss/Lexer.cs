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
                {   
                    new [] 
                    {
                        "int", "uint", "long",
                        "ulong", "short", "ushort",
                        "byte", "sbyte", "float",
                        "double", "decimal", "string"
                    }, TokenType.Primitive, LexerConstantPriority.High
                },

                {new Regex(@"-?\d+(\.\d+)?"), TokenType.Number},
                {new Regex(@"[a-zA-Z_][a-zA-Z\d_]*"), TokenType.Identifier, 2},
                {new Regex(@"""(([\r\n^""]|.|[^\\]"")*?[^\\])?""", RegexOptions.ExplicitCapture), TokenType.String}
            };
        }

        public static IEnumerable<Token<TokenType>> Lex(string inputString)
        {
            var reader = new StringeReader(inputString);
            // ReSharper disable once TooWideLocalVariableScope
            Token<TokenType> token;
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
        String
    }
}