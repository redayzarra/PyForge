namespace Compiler.Parts.Binding
{
    internal enum BoundNodeKind
    {
        #region Expressions
        LiteralExpression,
        UnaryExpression,
        VariableExpression,
        AssignmentExpression,
        #endregion

        #region Statements
        BlockStatement,
        ExpressionStatement,
        #endregion
    }
    
}