namespace Compiler.Parts.Binding
{
    internal sealed class BoundDivisionAssignmentExpression : BoundExpression
    {
        public BoundDivisionAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.DivisionAssignmentExpression;
        public override Type Type => Expression.Type;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    } 
}