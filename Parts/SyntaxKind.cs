namespace Compiler.Parts
{
    // Basically a list of all the things my compiler can recognize
    enum SyntaxKind
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