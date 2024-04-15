namespace Compiler.Parts.Syntax
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            return index >= _text.Length ? '\0' : _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            while (char.IsWhiteSpace(Current))
                Next();

            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

            var start = _position;
            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                    _diagnostics.Add($"The number {text} isn't a valid Int32");

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            // Handling single character tokens
            var current = Current;
            Next();
            var tokenKind = current switch
            {
                '+' => SyntaxKind.PlusToken,
                '-' => SyntaxKind.MinusToken,
                '*' => SyntaxKind.StarToken,
                '/' => SyntaxKind.SlashToken,
                '(' => SyntaxKind.OpenParenthesisToken,
                ')' => SyntaxKind.CloseParenthesisToken,
                _ => SyntaxKind.BadToken,
            };

            if (tokenKind == SyntaxKind.BadToken)
                _diagnostics.Add($"ERROR: Bad character in input: '{current}'");

            return new SyntaxToken(tokenKind, start, current.ToString(), null);
        }
    }
}