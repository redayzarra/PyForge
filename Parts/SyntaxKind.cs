namespace Compiler.Parts
{
    // Basically a list of all the things my compiler can recognize
    public enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression, 
        BinaryExpression,
        ParenthesizedExpression,
    }
}