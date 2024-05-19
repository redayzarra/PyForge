namespace Compiler.Parts.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        #region Arithmetic
        Addition,
        Subtraction,
        Multiplication,
        Division,
        #endregion

        #region Boolean
        LogicalAnd,
        LogicalOr,
        Equals,
        NotEquals,
        Identity,
        NonIdentity,
        #endregion

        #region Comparison
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals
        #endregion
    }
}
