namespace Compiler.Parts.Syntax
{
    public enum SyntaxKind
    {
        // Special tokens
        EndOfFileToken,
        BadToken,

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
        #endregion

        #region Punctuation Tokens
        OpenParenthesisToken,
        CloseParenthesisToken,
        #endregion

        #region Keywords
        TrueKeyword,
        FalseKeyword,
        NotKeyword,
        AndKeyword,
        OrKeyword,
        #endregion

        #region Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        #endregion

        #region Whitespace
        WhitespaceToken,
        #endregion
    }
}
