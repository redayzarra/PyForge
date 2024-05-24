namespace Compiler.Parts.Binding
{
    internal sealed class BoundMultiplicationAssignmentExpression : BoundExpression
    {
        public BoundMultiplicationAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.MultiplicationAssignmentExpression;
        public override Type Type => Expression.Type;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }    
}