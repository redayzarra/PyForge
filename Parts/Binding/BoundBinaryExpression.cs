namespace Compiler.Parts.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator operate, BoundExpression right)
        {
            Left = left;
            Operate = operate;
            Right = right;
        }

        public override Type Type => Left.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundExpression Left { get; }
        public BoundBinaryOperator Operate { get; }
        public BoundExpression Right { get; }
    }
    
}