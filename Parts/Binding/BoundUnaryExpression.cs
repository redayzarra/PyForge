namespace Compiler.Parts.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator operate, BoundExpression operand)
        {
            Operate = operate;
            Operand = operand;
        }

        public override Type Type => Operand.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundUnaryOperator Operate { get; }
        public BoundExpression Operand { get; }

    }
    
}