namespace Compiler.Parts.Syntax
{
    public enum SyntaxKind
    {
        // Special tokens
        BadToken,
        EndOfFileToken,

        #region Numerical Tokens
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        #endregion

        #region Punctuation Tokens
        OpenParenthesisToken,
        CloseParenthesisToken,
        #endregion

        #region Whitespace Tokens
        WhitespaceToken,
        #endregion

        #region Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        #endregion

        // Identifiers
        IdentifierToken, 

        #region Keywords
        TrueKeyword,
        FalseKeyword,
        NotKeyword,
        AndKeyword,
        OrKeyword,
        #endregion
    }
}
