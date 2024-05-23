using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class RangeExpressionSyntax : ExpressionSyntax
    {
        public RangeExpressionSyntax(SyntaxToken rangeKeyword, SyntaxToken openParenthesisToken, ExpressionSyntax lowerBound, SyntaxToken? commaToken1, ExpressionSyntax? upperBound, SyntaxToken? commaToken2, ExpressionSyntax? step, SyntaxToken closeParenthesisToken)
        {
            RangeKeyword = rangeKeyword;
            OpenParenthesisToken = openParenthesisToken;
            LowerBound = lowerBound;
            CommaToken1 = commaToken1;
            UpperBound = upperBound;
            CommaToken2 = commaToken2;
            Step = step;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public SyntaxToken RangeKeyword { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax LowerBound { get; }
        public SyntaxToken? CommaToken1 { get; }
        public ExpressionSyntax? UpperBound { get; }
        public SyntaxToken? CommaToken2 { get; }
        public ExpressionSyntax? Step { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override SyntaxKind Kind => SyntaxKind.RangeExpression;
    }
}