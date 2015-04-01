using System;
using System.Text.RegularExpressions;

using Stringes;

namespace LexerExample
{
    class Program
    {
        // Lexer
        private static readonly Lexer<M> lexer = new Lexer<M>
        {
            {"+", M.Plus},
            {"-", M.Minus},
            {"*", M.Asterisk},
            {"/", M.Slash},
            {"^", M.Caret},
            {"(", M.LeftParen},
            {")", M.RightParen},
            {new Regex(@"-?\d+(\.\d+)?"), M.Number},
            {new Regex(@"\s+"), M.Whitespace}
        }.Ignore(M.Whitespace);

        // Token types
        enum M
        {
            Plus,
            Minus,
            Asterisk,
            Slash,
            Caret,
            LeftParen,
            RightParen,
            Number,
            Whitespace
        }

        static void Main(string[] args)
        {
            Console.Title = "Stringes Lexer Example";

            var origText = "2 * 3 / (5 + 1) ^ 2";

            Console.WriteLine("ORIGINAL:\n");
            Console.WriteLine(origText);
            Console.WriteLine("\nTOKENS:\n");

            foreach (var token in lexer.Tokenize(origText))
            {
                Console.WriteLine(token);
            }

            Console.ReadKey();
        }
    }
}
