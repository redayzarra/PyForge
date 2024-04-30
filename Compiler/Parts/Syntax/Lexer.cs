namespace Compiler.Parts.Syntax
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        private SyntaxKind _kind;
        private int _start;
        private object? _value;

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

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
            _start = _position;
            while (condition(Current))
                Next();
            return _text.Substring(_start, _position - _start);
        }

        public SyntaxToken Lex()
        {
            int start = _position;

            // List of functions to check for each type of token
            var tokenChecks = new List<Func<SyntaxToken?>>()
            {
                CheckForWhitespace,
                CheckForEqualityOperators,
                CheckForSingleCharacterOperators,
                CheckForLetters,
                CheckForDigits,
                CheckForSpecialCharacters 
            };

            // Iterate through each check and return the first non-null token found
            foreach (var check in tokenChecks)
            {
                var token = check();
                if (token != null)
                    return token;
            }

            // If no token is found, return a bad token
            return new SyntaxToken(SyntaxKind.BadToken, start, "", null);
        }

        private SyntaxToken? CheckForWhitespace()
        {
            if (char.IsWhiteSpace(Current))
            {
                var starting = _position;
                var whitespace = ConsumeWhile(char.IsWhiteSpace);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, starting, whitespace, null);
            }
            return null;
        }

        private SyntaxToken? CheckForEqualityOperators()
        {
            if (Current == '=' && LookAhead == '=')
            {
                _position += 2; 
                return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _start, "==", null);
            }
            if (Current == '!' && LookAhead == '=')
            {
                _position += 2;
                return new SyntaxToken(SyntaxKind.NotEqualsToken, _start, "!=", null);
            }
            if (Current == '=' && LookAhead != '=')
            {
                Next();
                return new SyntaxToken(SyntaxKind.EqualsToken, _start, "=", null);
            }
            return null;
        }

        private SyntaxToken? CheckForSingleCharacterOperators()
        {
            char currentChar = Current;
            SyntaxKind tokenKind = currentChar switch
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
                Next(); // Advance the position for single character token
                return new SyntaxToken(tokenKind, _start, currentChar.ToString(), null);
            }

            return null;
        }

        private SyntaxToken? CheckForLetters()
        {
            if (char.IsLetter(Current))
            {
                var text = ConsumeWhile(char.IsLetter);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, _start, text, null);
            }
            return null;
        }

        private SyntaxToken? CheckForDigits()
        {
            if (char.IsDigit(Current))
            {
                var text = ConsumeWhile(char.IsDigit);
                var length = _position - _start;  // Calculate the length
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), _text, typeof(int));
                    return new SyntaxToken(SyntaxKind.NumberToken, _start, text, null);
                }

                _value = value;
                _kind = SyntaxKind.NumberToken;
                return new SyntaxToken(_kind, _start, text, value);
            }
            return null;
        }

        private SyntaxToken? CheckForSpecialCharacters()
        {
            if (Current == '\0')
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            _diagnostics.ReportBadCharacter(_position, Current);
            var result = new SyntaxToken(SyntaxKind.BadToken, _start, Current.ToString(), null);
            Next(); // Consume the bad character
            return result;
        }
    }
}
