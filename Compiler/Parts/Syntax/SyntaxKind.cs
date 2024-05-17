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
        #endregion

        #region Punctuation Tokens
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        #endregion

        #region Keywords
        TrueKeyword,
        FalseKeyword,
        NotKeyword,
        AndKeyword,
        OrKeyword,
        IsKeyword,
        IsNotKeyword,
        #endregion

        #region Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression,
        #endregion

        #region Whitespace
        WhitespaceToken,
        #endregion

        #region Statements
        BlockStatement,
        ExpressionStatement,
        #endregion
    }
}
