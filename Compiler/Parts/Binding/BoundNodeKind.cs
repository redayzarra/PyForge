namespace Compiler.Parts.Binding
{
    internal enum BoundNodeKind
    {
        #region Expressions
        LiteralExpression,
        UnaryExpression,
        VariableExpression,
        AssignmentExpression,
        AdditionAssignmentExpression,
        SubtractionAssignmentExpression,
        MultiplicationAssignmentExpression,
        DivisionAssignmentExpression,
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