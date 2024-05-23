using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class ElseClauseSyntax : SyntaxNode
    {
        public ElseClauseSyntax(SyntaxToken elseKeyword, SyntaxToken colonToken, StatementSyntax elseStatement)
        {
            ElseKeyword = elseKeyword;
            ColonToken = colonToken;
            ElseStatement = elseStatement;
        }

        public override SyntaxKind Kind => SyntaxKind.ElseClause;

        public SyntaxToken ElseKeyword { get; }
        public SyntaxToken ColonToken { get; }
        public StatementSyntax ElseStatement { get; }
    }
}