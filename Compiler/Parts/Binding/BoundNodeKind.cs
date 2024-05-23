namespace Compiler.Parts.Binding
{
    internal enum BoundNodeKind
    {
        #region Expressions
        LiteralExpression,
        UnaryExpression,
        VariableExpression,
        AssignmentExpression,
        RangeExpression,
        #endregion

        #region Statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        #endregion
    }
}