namespace Compiler.Parts
{
    internal static class SyntaxFacts
    {
        // Get the precedence of the unary operators (e.g. -1 + 2)
        internal static int GetUnaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;
                
                default:
                    return 0;
            }
        }

        // Get the precedence of operators (PEMDAS)
        internal static int GetBinaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                
                default:
                    return 0;
            }
        }
    }
}