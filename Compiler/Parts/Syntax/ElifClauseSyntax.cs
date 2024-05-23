namespace Compiler.Parts.Syntax
{
    public sealed class ElifClauseSyntax : SyntaxNode
    {
        public ElifClauseSyntax(SyntaxToken elifKeyword, ExpressionSyntax condition, SyntaxToken colonToken, StatementSyntax statement)
        {
            ElifKeyword = elifKeyword;
            Condition = condition;
            ColonToken = colonToken;
            Statement = statement;
        }

        public override SyntaxKind Kind => SyntaxKind.ElifClause;

        public SyntaxToken ElifKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken ColonToken { get; }
        public StatementSyntax Statement { get; }
    }
}
