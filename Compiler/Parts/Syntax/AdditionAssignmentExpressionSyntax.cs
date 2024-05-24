using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class AdditionAssignmentExpressionSyntax : ExpressionSyntax
    {
        public AdditionAssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken plusEqualsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            PlusEqualsToken = plusEqualsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken PlusEqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AdditionAssignmentExpression;
    }
}