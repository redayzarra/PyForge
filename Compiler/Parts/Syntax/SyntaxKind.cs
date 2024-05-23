namespace Compiler.Parts.Syntax
{
    public enum SyntaxKind
    {
        #region Compilation Unit
        CompilationUnit,
        #endregion

        #region Special Tokens
        EndOfFileToken,
        BadToken,
        #endregion

        #region Literal Tokens
        NumberToken,
        IdentifierToken,
        #endregion

        #region Operator Tokens
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        EqualsEqualsToken,
        NotEqualsToken,
        EqualsToken,
        GreaterThanOrEqualsToken,
        LessThanOrEqualsToken,
        GreaterThanToken,
        LessThanToken,
        #endregion

        #region Punctuation Tokens
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        CommaToken,
        #endregion

        #region Keywords
        TrueKeyword,
        FalseKeyword,
        NotKeyword,
        AndKeyword,
        OrKeyword,
        IsKeyword,
        IsNotKeyword,
        IfKeyword,
        ElifKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        InKeyword,
        RangeKeyword,
        #endregion

        #region Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression,
        RangeExpression,
        #endregion

        #region Whitespace
        WhitespaceToken,
        #endregion

        #region Statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        ElifClause,
        ElseClause,
        WhileStatement,
        ForStatement,
        #endregion
    }
}
