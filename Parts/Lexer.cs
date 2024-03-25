namespace Compiler.Parts
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => _position >= _text.Length ? '\0' : _text[_position];

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // Skip any whitespace
            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            // After skipping whitespaces, check if we're at the end of the text
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

            // If the char is a number, then grab the entire number and return it
            if (char.IsDigit(Current))
            {
                // From our current position, let's try extracting the num
                var start = _position;
                
                // Keep increasing the pointer to get the whole number
                while (char.IsDigit(Current))
                    Next();
                
                // Calculate the length of the text and the text itself
                var length = _position - start;
                var text = _text.Substring(start, length);

                // Error handling if the number can't be converted to int
                if (!int.TryParse(text, out var value))
                    _diagnostics.Add($"The number {_text} isn't a valid Int32");
            
                // Otherwise, it's valid and we are good to go
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
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

            // If we recognize the special token (operator), then return it
            if (tokenKind != SyntaxKind.BadToken)
            {
                // Consume the character for a recognized token
                var tokenText = _text.Substring(_position, 1);
                _position++;
                return new SyntaxToken(tokenKind, _position - 1, tokenText, null);
            }

            // Otherwise, skip the unrecognized character and return a bad token
            _diagnostics.Add($"ERROR: Bad character in input: '{Current}'");
            Next();
            return new SyntaxToken(SyntaxKind.BadToken, _position - 1, _text.Substring(_position - 1, 1), null);
        } 
    } 
}