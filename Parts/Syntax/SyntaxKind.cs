namespace Compiler.Parts.Syntax
{
    // Basically a list of all the things my compiler can recognize
    public enum SyntaxKind
    {
        // Special tokens
        BadToken,
        EndOfFileToken,

        // Numerical tokens
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,

        // Text tokens
        WhitespaceToken,
        OpenParenthesisToken,
        CloseParenthesisToken,

        // Expressions
        LiteralExpression, 
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}