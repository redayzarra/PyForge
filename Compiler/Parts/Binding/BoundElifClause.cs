namespace Compiler.Parts.Binding
{
    internal sealed class BoundElifClause
    {
        public BoundElifClause(BoundExpression condition, BoundStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
    }
}
