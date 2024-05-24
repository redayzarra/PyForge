using Compiler.Parts.Text;

namespace Compiler.Parts.Syntax
{
    // Analyzes text and breaks it down into syntax tokens for parsing
    internal sealed class Lexer
    {
        private readonly SourceText _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;
        private SyntaxKind _kind;
        private int _start;
        private object? _value;

        public Lexer(SourceText text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

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

        private string ConsumeWhile(Func<char, bool> condition)
        {
            _start = _position;
            while (condition(Current))
                Next();
            return _text.ToString(_start, _position - _start);
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
                HandleWhitespace();
            else if (Current == '=')
                HandleEquals();
            else if (Current == '+')
                HandlePlus();
            else if (Current == '-')
                HandleMinus();
            else if (Current == '*')
                HandleStar();
            else if (Current == '/')
                HandleSlash();
            else if (Current == '!')
                HandleExclamation();
            else if (Current == '>' || Current == '<')
                HandleComparisonOperators();
            else if (char.IsLetter(Current))
                HandleIdentifier();
            else if (char.IsDigit(Current))
                HandleNumber();
            else
                HandleSingleCharacterToken();

            string textRepresentation = _value?.ToString() ?? string.Empty;
            return new SyntaxToken(_kind, _start, textRepresentation, _value);
        }

        private void HandleWhitespace()
        {
            var whitespace = ConsumeWhile(char.IsWhiteSpace);
            SetToken(SyntaxKind.WhitespaceToken, whitespace);
        }

        private void HandleEquals()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.EqualsEqualsToken, "==");
            }
            else
            {
                Next();
                SetToken(SyntaxKind.EqualsToken, "=");
            }
        }

        private void HandleExclamation()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.NotEqualsToken, "!=");
            }
            else
            {
                _diagnostics.ReportBadCharacter(_position, Current);
                SetToken(SyntaxKind.BadToken);
                Next();
            }
        }

        private void HandleComparisonOperators()
        {
            if (Current == '>' && LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.GreaterThanOrEqualsToken, ">=");
            }
            else if (Current == '<' && LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.LessThanOrEqualsToken, "<=");
            }
            else if (Current == '>')
            {
                Next();
                SetToken(SyntaxKind.GreaterThanToken, ">");
            }
            else if (Current == '<')
            {
                Next();
                SetToken(SyntaxKind.LessThanToken, "<");
            }
        }

        private void HandleIdentifier()
        {
            var text = ConsumeWhile(char.IsLetter);
            SetToken(SyntaxFacts.GetKeywordKind(text), text);
        }

        private void HandleNumber()
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

        private void HandleSingleCharacterToken()
        {
            switch (Current)
            {
                case '\0':
                    SetToken(SyntaxKind.EndOfFileToken);
                    break;
                case '(':
                    SetToken(SyntaxKind.OpenParenthesisToken);
                    break;
                case ')':
                    SetToken(SyntaxKind.CloseParenthesisToken);
                    break;
                case '{':
                    SetToken(SyntaxKind.OpenBraceToken);
                    break;
                case '}':
                    SetToken(SyntaxKind.CloseBraceToken);
                    break;
                case ',':
                    SetToken(SyntaxKind.CommaToken);
                    break;
                case ':':
                    SetToken(SyntaxKind.ColonToken);
                    break;
                default:
                    _diagnostics.ReportBadCharacter(_position, Current);
                    SetToken(SyntaxKind.BadToken);
                    break;
            }
            _value = Current.ToString();
            Next();
        }

        private void HandlePlus()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.PlusEqualsToken, "+=");
            }
            else
            {
                SetToken(SyntaxKind.PlusToken, "+");
                Next(); // Move past the '+' character
            }
        }

        private void HandleMinus()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.MinusEqualsToken, "-=");
            }
            else
            {
                SetToken(SyntaxKind.MinusToken, "-");
                Next(); // Move past the '-' character
            }
        }

        private void HandleStar()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.StarEqualsToken, "*=");
            }
            else
            {
                SetToken(SyntaxKind.StarToken, "*");
                Next(); // Move past the '+' character
            }
        }

        private void HandleSlash()
        {
            if (LookAhead == '=')
            {
                _position += 2;
                SetToken(SyntaxKind.SlashEqualsToken, "/=");
            }
            else
            {
                SetToken(SyntaxKind.SlashToken, "/");
                Next(); // Move past the '+' character
            }
        }
    }
}
