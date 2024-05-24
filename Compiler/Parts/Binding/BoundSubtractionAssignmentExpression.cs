namespace Compiler.Parts.Binding
{
    internal sealed class BoundSubtractionAssignmentExpression : BoundExpression
    {
        public BoundSubtractionAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.SubtractionAssignmentExpression;
        public override Type Type => Expression.Type;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}