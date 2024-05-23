using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxToken whileKeyword, ExpressionSyntax condition, SyntaxToken colonToken, StatementSyntax body)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            ColonToken = colonToken;
            Body = body;
        }

        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken ColonToken { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}