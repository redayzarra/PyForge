using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class MultiplicationAssignmentExpressionSyntax : ExpressionSyntax
    {
        public MultiplicationAssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken starEqualsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            StarEqualsToken = starEqualsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken StarEqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.MultiplicationAssignmentExpression;
    }
}