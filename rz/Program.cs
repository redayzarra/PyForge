using System;

namespace rz
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please enter an expression (or press Enter to exit):");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;
                
                if (line == "4 + 5")
                    Console.WriteLine("7");
                else
                {
                    Console.WriteLine("ERROR: Invalid Expression");
                }
            }
        }
    }

    enum SyntaxKind {}

    class SyntaxToken {
        public SyntaxToken(SyntaxKind kind, int position, string text) {

        }
    }

    class Lexer {
        private readonly string _text;
        private int _position;

        public Lexer(string text) {
            _text = text;
        }

        public SyntaxToken NextToken() {}
    }
}
