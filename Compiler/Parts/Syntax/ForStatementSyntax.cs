using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax rangeExpression, StatementSyntax body)
        {
            ForKeyword = forKeyword;
            Identifier = identifier;
            InKeyword = inKeyword;
            RangeExpression = rangeExpression;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken InKeyword { get; }
        public ExpressionSyntax RangeExpression { get; }
        public StatementSyntax Body { get; }
    }
}

