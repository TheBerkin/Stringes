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
            // Constant rules
            {"+", M.Plus},
            {"-", M.Minus},
            {"*", M.Asterisk},
            {"/", M.Slash},
            {"^", M.Caret},
            {"(", M.LeftParen},
            {")", M.RightParen},

            // Function rule
            {
                reader =>
                {
                    reader.Eat('-');
                    if (!reader.EatWhile(Char.IsDigit)) return false;
                    return !reader.Eat('.') || reader.EatWhile(Char.IsDigit);
                },
                M.Number
            },

            // Regex rule
            {new Regex(@"\s"), M.Whitespace}
        }
        .Ignore(M.Whitespace);

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

            var origText = "20 * 3.14 / (5 + 11) ^ 2";

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
