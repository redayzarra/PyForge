namespace Compiler.Parts.Binding
{
    internal sealed class BoundAdditionAssignmentExpression : BoundExpression
    {
        public BoundAdditionAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AdditionAssignmentExpression;
        public override Type Type => Expression.Type;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}