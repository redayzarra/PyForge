using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class DivisionAssignmentExpressionSyntax : ExpressionSyntax
    {
        public DivisionAssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken slashEqualsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            SlashEqualsToken = slashEqualsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken SlashEqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.DivisionAssignmentExpression;
    }
}