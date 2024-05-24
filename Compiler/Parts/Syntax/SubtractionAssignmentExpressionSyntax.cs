using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class SubtractionAssignmentExpressionSyntax : ExpressionSyntax
    {
        public SubtractionAssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken minusEqualsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            MinusEqualsToken = minusEqualsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken MinusEqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.SubtractionAssignmentExpression;
    }
}