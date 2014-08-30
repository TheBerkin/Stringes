using System;
using System.Linq;

namespace LexMyAss
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("lexer> ");
                var code = Console.ReadLine();
                try
                {
                    var tokens = Lexer.Lex(code);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(tokens.Select(t => t.ToString()).Aggregate((c, n) => c + "\n" + n));
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
