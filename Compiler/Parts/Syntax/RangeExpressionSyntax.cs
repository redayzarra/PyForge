using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class RangeExpressionSyntax : ExpressionSyntax
    {
        public RangeExpressionSyntax(SyntaxToken identifier, SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            Identifier = identifier;
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.RangeExpression;

        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }
    }
}