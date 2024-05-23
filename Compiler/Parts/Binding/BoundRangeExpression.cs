namespace Compiler.Parts.Binding
{
    internal sealed class BoundRangeExpression : BoundExpression
    {
        public BoundRangeExpression(BoundExpression lowerBound, BoundExpression upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public override BoundNodeKind Kind => BoundNodeKind.RangeExpression;

        public override Type Type => typeof(int);
        public BoundExpression LowerBound { get; }
        public BoundExpression UpperBound { get; }
    }
}