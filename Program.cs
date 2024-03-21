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

                var lexer = new Lexer(line);
                while (true)
                {
                    var token = lexer.NextToken();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;

                    Console.WriteLine($"{token.Kind}: '{token.Text}'");
                    if (token.Value != null)
                        Console.WriteLine($"{token.Value}");
                    
                    Console.WriteLine();
                }
            }
        }
    }

    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
    }

    class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current => _position >= _text.Length ? '\0' : _text[_position];

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // Skip any whitespace
            while (char.IsWhiteSpace(Current))
                Next();

            // After skipping whitespace, check if we're at the end of the text
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

            // If the char is a number, then grab the entire number and return it
            if (char.IsDigit(Current))
            {
                var start = _position;
                
                while (char.IsDigit(Current))
                    Next();
                
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (int.TryParse(text, out var value))
                {
                    return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
                }
            }

            // Handling operators and parentheses
            var tokenKind = Current switch
            {
                '+' => SyntaxKind.PlusToken,
                '-' => SyntaxKind.MinusToken,
                '*' => SyntaxKind.StarToken,
                '/' => SyntaxKind.SlashToken,
                '(' => SyntaxKind.OpenParenthesisToken,
                ')' => SyntaxKind.CloseParenthesisToken,
                _ => SyntaxKind.BadToken,
            };

            if (tokenKind != SyntaxKind.BadToken)
            {
                // Consume the character for a recognized token
                var tokenText = _text.Substring(_position, 1);
                _position++;
                return new SyntaxToken(tokenKind, _position - 1, tokenText, null);
            }

            // Skip the unrecognized character and return a bad token
            Next();
            return new SyntaxToken(SyntaxKind.BadToken, _position - 1, _text.Substring(_position - 1, 1), null);
        } 
    }
}

