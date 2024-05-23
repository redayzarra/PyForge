using Compiler.Parts.Syntax;
using System.Collections.Immutable;

namespace Compiler.Parts
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken colonToken, StatementSyntax thenStatement, ImmutableArray<ElifClauseSyntax> elifClauses, ElseClauseSyntax? elseClause)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ColonToken = colonToken;
            ThenStatement = thenStatement;
            ElifClauses = elifClauses;
            ElseClause = elseClause;
        }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken ColonToken { get; }
        public StatementSyntax ThenStatement { get; }
        public ImmutableArray<ElifClauseSyntax> ElifClauses { get; }
        public ElseClauseSyntax? ElseClause { get; }
    }
}

