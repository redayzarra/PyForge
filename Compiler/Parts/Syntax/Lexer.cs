namespace Compiler.Parts.Syntax
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        private string ConsumeWhile(Func<char, bool> condition)
        {
            var start = _position;
            while (condition(Current))
                Next();
            return _text.Substring(start, _position - start);
        }

        public SyntaxToken Lex()
        {
            while (char.IsWhiteSpace(Current))
                Next();

            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

            var start = _position;

            // Check for two-character tokens
            if (Current == '=' && LookAhead == '=')
            {
                _position += 2; 
                return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
            }

            if (Current == '!' && LookAhead == '=')
            {
                _position += 2;
                return new SyntaxToken(SyntaxKind.NotEqualsToken, start, "!=", null);
            }

            if (char.IsLetter(Current))
            {
                var text = ConsumeWhile(char.IsLetter);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            if (char.IsDigit(Current))
            {
                var text = ConsumeWhile(char.IsDigit);
                var length = _position - start;  // Calculate the length
                if (!int.TryParse(text, out var value))
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            var currentChar = Current;  // Save the current character before potentially calling Next()
            var tokenKind = currentChar switch
            {
                '+' => SyntaxKind.PlusToken,
                '-' => SyntaxKind.MinusToken,
                '*' => SyntaxKind.StarToken,
                '/' => SyntaxKind.SlashToken,
                '(' => SyntaxKind.OpenParenthesisToken,
                ')' => SyntaxKind.CloseParenthesisToken,
                _ => SyntaxKind.BadToken,
            };

            Next(); // Advance the position for single character token

            if (tokenKind == SyntaxKind.BadToken)
                _diagnostics.ReportBadCharacter(_position, Current);

            return new SyntaxToken(tokenKind, start, currentChar.ToString(), null);
        }
    }
}
