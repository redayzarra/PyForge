using System.Collections.Immutable;
using Compiler.Parts.Text;
using static Compiler.Parts.Syntax.SyntaxFacts;

namespace Compiler.Parts.Syntax
{
    // Uses the tokens from the Lexer to create a syntax tree
    internal sealed class Parser 
    {
        private int _position;
        private readonly SourceText _text;
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        // Creates a list of tokens from a given text
        public Parser(SourceText text)
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

            _text = text;
            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

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
            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, "", null);
        }

        // Builds binary expressions for "+" and "-" operators
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private StatementSyntax ParseForStatement()
        {
            var forKeyword = MatchToken(SyntaxKind.ForKeyword);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var inKeyword = MatchToken(SyntaxKind.InKeyword);
            var rangeExpression = ParseExpression();
            var body = ParseStatement();
            return new ForStatementSyntax(forKeyword, identifier, inKeyword, rangeExpression, body);
        }

        private StatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();

            var body = ParseStatement();
            return new WhileStatementSyntax(keyword, condition, body);
        }

        private StatementSyntax ParseIfStatement()
        {
            var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var thenStatement = ParseStatement();
            var elifClauses = ParseElifClauses();
            var elseClause = ParseElseClause();

            return new IfStatementSyntax(ifKeyword, condition, thenStatement, elifClauses, elseClause);
        }

        private ImmutableArray<ElifClauseSyntax> ParseElifClauses()
        {
            var elifClauses = ImmutableArray.CreateBuilder<ElifClauseSyntax>();

            while (Current.Kind == SyntaxKind.ElifKeyword)
            {
                var elifKeyword = MatchToken(SyntaxKind.ElifKeyword);
                var condition = ParseExpression();
                var statement = ParseStatement();
                var elifClause = new ElifClauseSyntax(elifKeyword, condition, statement);
                elifClauses.Add(elifClause);
            }

            return elifClauses.ToImmutable();
        }

        private ElseClauseSyntax? ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
                return null;
            
            var keyword = NextToken();
            var statement = ParseStatement();
            return new ElseClauseSyntax(keyword, statement);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.EndOfFileToken &&
                   Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = GetUnaryOperator(Current.Kind);

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = GetBinaryOperator(Current.Kind);
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();

                // Handle compound binary operators like "is not"
                if (operatorToken.Kind == SyntaxKind.IsKeyword && Peek(0).Kind == SyntaxKind.NotKeyword)
                {
                    NextToken(); // Consume "not" token
                    // Combine "is" and "not" into a new compound operator token
                    operatorToken = new SyntaxToken(SyntaxKind.IsNotKeyword, operatorToken.Position, "is not", null);
                    precedence = GetBinaryOperator(SyntaxKind.IsNotKeyword); // Update precedence for compound operator
                }

                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        // Parses a primary expression (simplest form) into syntax node
        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParenthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();

                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();

                case SyntaxKind.RangeKeyword:
                    return ParseRangeExpression();

                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseRangeExpression()
        {
            var rangeKeyword = MatchToken(SyntaxKind.RangeKeyword);
            var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);

            var lowerBound = ParseExpression();

            SyntaxToken? commaToken1 = null;
            ExpressionSyntax? upperBound = null;
            SyntaxToken? commaToken2 = null;
            ExpressionSyntax? step = null;

            if (Current.Kind == SyntaxKind.CommaToken)
            {
                commaToken1 = MatchToken(SyntaxKind.CommaToken);
                upperBound = ParseExpression();

                if (Current.Kind == SyntaxKind.CommaToken)
                {
                    commaToken2 = MatchToken(SyntaxKind.CommaToken);
                    step = ParseExpression();
                }
            }

            var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);

            return new RangeExpressionSyntax(rangeKeyword, openParenthesisToken, lowerBound, commaToken1, upperBound, commaToken2, step, closeParenthesisToken);
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesisToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }
    }
}