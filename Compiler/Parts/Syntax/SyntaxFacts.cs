namespace Compiler.Parts.Syntax
{
    public static class SyntaxFacts
    {
        private static readonly SyntaxKind[] _allKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().ToArray();

        public static int GetUnaryOperator(SyntaxKind kind) => kind switch
        {
            // Higher precedence for unary operators
            SyntaxKind.PlusToken => 6,
            SyntaxKind.MinusToken => 6,
            SyntaxKind.NotKeyword => 6,
            _ => 0
        };

        public static int GetBinaryOperator(SyntaxKind kind) => kind switch
        {
            // Multiplication and division have high precedence
            SyntaxKind.StarToken => 5,
            SyntaxKind.SlashToken => 5,

            // Addition and subtraction have medium precedence
            SyntaxKind.PlusToken => 4,
            SyntaxKind.MinusToken => 4,

            // Equality and inequality have specific precedence
            SyntaxKind.EqualsEqualsToken => 3,
            SyntaxKind.NotEqualsToken => 3,
            SyntaxKind.IsKeyword => 3,
            SyntaxKind.IsNotKeyword => 3,

            // Logical AND has lower precedence than equality checks
            SyntaxKind.AndKeyword => 2,

            // Logical OR has the lowest precedence among binary 
            SyntaxKind.OrKeyword => 1,
            _ => 0
        };

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds() =>
            _allKinds.Where(kind => GetBinaryOperator(kind) > 0);

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds() =>
            _allKinds.Where(kind => GetUnaryOperator(kind) > 0);

        public static SyntaxKind GetKeywordKind(string text) => text switch
        {
            "True" => SyntaxKind.TrueKeyword,
            "False" => SyntaxKind.FalseKeyword,
            "and" => SyntaxKind.AndKeyword,
            "or" => SyntaxKind.OrKeyword,
            "not" => SyntaxKind.NotKeyword,
            "is" => SyntaxKind.IsKeyword,
            _ => SyntaxKind.IdentifierToken
        };

        public static string? GetText(SyntaxKind kind) => kind switch
        {
            SyntaxKind.PlusToken => "+",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.StarToken => "*",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.NotEqualsToken => "!=",
            SyntaxKind.OpenParenthesisToken => "(",
            SyntaxKind.CloseParenthesisToken => ")",
            SyntaxKind.OpenBraceToken => "{",
            SyntaxKind.CloseBraceToken => "}",
            SyntaxKind.AndKeyword => "and",
            SyntaxKind.OrKeyword => "or",
            SyntaxKind.NotKeyword => "not",
            SyntaxKind.IsKeyword => "is",
            SyntaxKind.IsNotKeyword => "is not",
            SyntaxKind.TrueKeyword => "True",
            SyntaxKind.FalseKeyword => "False",
            _ => null
        };
    }
}

