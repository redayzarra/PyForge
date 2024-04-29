namespace Compiler.Parts.Syntax
{
    public static class SyntaxFacts
    {
        // Get the precedence of unary operators (e.g., -1 + 2)
        public static int GetUnaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken: 
                case SyntaxKind.MinusToken: 
                case SyntaxKind.NotKeyword: 
                    return 6; // Higher precedence for unary operators
                default:
                    return 0;
            }
        }

        // Get the precedence of binary operators (PEMDAS and logical)
        public static int GetBinaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 5; // Multiplication and division have high precedence

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4; // Addition and subtraction have medium precedence

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.NotEqualsToken:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.IsNotKeyword:
                    return 3; // Equality and inequality have specific precedence

                case SyntaxKind.AndKeyword: 
                    return 2; // Logical AND has lower precedence than equality checks

                case SyntaxKind.OrKeyword: 
                    return 1; // Logical OR has the lowest precedence among binary operators

                default:
                    return 0;
            }
        }

        // Map text to SyntaxKind for keywords and logical operators
        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "True":
                    return SyntaxKind.TrueKeyword;
                case "False":
                    return SyntaxKind.FalseKeyword;
                case "and":
                    return SyntaxKind.AndKeyword;
                case "or":
                    return SyntaxKind.OrKeyword;
                case "not":
                    return SyntaxKind.NotKeyword;
                case "is":
                    return SyntaxKind.IsKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static string? GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.NotEqualsToken:
                    return "!=";
                case SyntaxKind.AndKeyword:
                    return "and";
                case SyntaxKind.OrKeyword:
                    return "or";
                case SyntaxKind.NotKeyword:
                    return "not";
                case SyntaxKind.IsKeyword:
                    return "is";
                case SyntaxKind.IsNotKeyword:
                    return "is not";
                case SyntaxKind.TrueKeyword:
                    return "True";
                case SyntaxKind.FalseKeyword:
                    return "False";
                default:
                    return null;
            }
        }
    }
}
