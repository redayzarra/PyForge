using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    // for i = 1 to 10
    // {

    // }
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax lowerBound, ExpressionSyntax upperBound)
        {
            
        }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;
    }
}

