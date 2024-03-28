namespace Compiler.Parts
{
    // Uses the tokens from the Lexer to create a syntax tree
    internal sealed class Parser 
    {
        private readonly SyntaxToken[] _tokens;
        private List<string> _diagnostics = new List<string>();
        private int _position;

        // Creates a list of tokens from a given text
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;

            do
            { // Add valid tokens to the _tokens array
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhitespaceToken && 
                    token.Kind != SyntaxKind.BadToken) 
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        // Given a certain offset, look at the token at that index
        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index < 0 || index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            
            return _tokens[index];
        }

        // Current is the token at the current position
        private SyntaxToken Current => Peek(0);

        // Returns the current token, then moves to next token
        private SyntaxToken NextToken()
        {
            // Return the same token we started with, but shift to next
            var current = Current;
            _position++;
            return current;
        }

        // If the token matches what it's suppose to be, move on or create null node
        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            // If the current token is the same kind, return the current
            if (Current.Kind == kind)
                return NextToken(); // But move the Parser's focus to next

            // Otherwise, return a placeholder token with null content
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, "", null);
        }

        // Builds binary expressions for "+" and "-" operators
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        private ExpressionSyntax ParseTerm()
        {
            // Start with the first part of the expression (left side)
            var left = ParseFactor();

            // Keep going as long as we see "+" or "-" operators
            while (Current.Kind == SyntaxKind.PlusToken || 
                   Current.Kind == SyntaxKind.MinusToken)
            {
                // Capture the operator and combine the left and right sides
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            // Return the constructed expression (can be number or binary)
            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            // Start with the first part of the expression (left side)
            var left = ParsePrimaryExpression();

            // Keep going as long as we see "*" or "/" operators
            while (Current.Kind == SyntaxKind.StarToken ||
                   Current.Kind == SyntaxKind.SlashToken)
            {
                // Capture the operator and combine the left and right sides
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            // Return the constructed expression (can be number or binary)
            return left;
        }

        // Parses a primary expression (simplest form) into syntax node
        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}