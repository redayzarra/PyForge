using System.Collections.Immutable;

namespace Compiler.Parts.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement thenStatement, ImmutableArray<BoundElifClause> elifClauses, BoundStatement? elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElifClauses = elifClauses;
            ElseStatement = elseStatement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public ImmutableArray<BoundElifClause> ElifClauses { get; }
        public BoundStatement? ElseStatement { get; }
    }
}
