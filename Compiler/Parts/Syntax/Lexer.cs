using Compiler.Parts.Text;

namespace Compiler.Parts.Syntax
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    internal sealed class Lexer
    {
        private readonly string _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;
        private SyntaxKind _kind;
        private int _start;
        private object? _value;

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

        private void SetToken(SyntaxKind kind, object? value = null)
        {
            _kind = kind;
            _value = value;
        }

        public SyntaxToken Lex()
        {
            _start = _position; 

            if (char.IsWhiteSpace(Current))
            {
                var whitespace = ConsumeWhile(char.IsWhiteSpace);
                SetToken(SyntaxKind.WhitespaceToken, whitespace);
            }
            else if (Current == '=' && LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.EqualsEqualsToken, "==");
            }
            else if (Current == '!' && LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.NotEqualsToken, "!=");
            }
            else if (Current == '=' && LookAhead != '=')
            {
                Next();
                SetToken(SyntaxKind.EqualsToken, "=");
            }
            else if (char.IsLetter(Current))
            {
                var text = ConsumeWhile(char.IsLetter);
                SetToken(SyntaxFacts.GetKeywordKind(text), text);
            }
            else if (char.IsDigit(Current))
            {
                var text = ConsumeWhile(char.IsDigit);
                if (int.TryParse(text, out var value))
                {
                    SetToken(SyntaxKind.NumberToken, value);
                }
                else
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(_start, text.Length), text, typeof(int));
                    SetToken(SyntaxKind.NumberToken, null);
                }
            }
            else
            {
                switch (Current)
                {
                    case '\0':
                        SetToken(SyntaxKind.EndOfFileToken);
                        break;
                    case '+':
                        SetToken(SyntaxKind.PlusToken);
                        break;
                    case '-':
                        SetToken(SyntaxKind.MinusToken);
                        break;
                    case '*':
                        SetToken(SyntaxKind.StarToken);
                        break;
                    case '/':
                        SetToken(SyntaxKind.SlashToken);
                        break;
                    case '(':
                        SetToken(SyntaxKind.OpenParenthesisToken);
                        break;
                    case ')':
                        SetToken(SyntaxKind.CloseParenthesisToken);
                        break;
                    default:
                        _diagnostics.ReportBadCharacter(_position, Current);
                        SetToken(SyntaxKind.BadToken);
                        break;
                }
                _value = Current.ToString();
                Next(); // Advance the position for single character token
            }

            // Ensuring text is not null when creating a SyntaxToken
            string textRepresentation = _value?.ToString() ?? string.Empty;
            return new SyntaxToken(_kind, _start, textRepresentation, _value);
        }
    }
}
