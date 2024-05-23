namespace Compiler.Parts.Binding
{
    internal sealed class BoundRangeExpression : BoundExpression
    {
        public BoundRangeExpression(BoundExpression lowerBound, BoundExpression? upperBound, BoundExpression? step)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Step = step;
        }

        public BoundExpression LowerBound { get; }
        public BoundExpression? UpperBound { get; }
        public BoundExpression? Step { get; }

        public override Type Type => typeof(int[]);
        public override BoundNodeKind Kind => BoundNodeKind.RangeExpression;
    }
}