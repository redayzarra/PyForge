namespace Compiler.Parts.Syntax
{
    public sealed class ElifClauseSyntax : SyntaxNode
    {
        public ElifClauseSyntax(SyntaxToken elifKeyword, ExpressionSyntax condition, StatementSyntax statement)
        {
            ElifKeyword = elifKeyword;
            Condition = condition;
            Statement = statement;
        }

        public override SyntaxKind Kind => SyntaxKind.ElifClause;

        public SyntaxToken ElifKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Statement { get; }
    }
}
