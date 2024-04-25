using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexes_Token(SyntaxKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text);

        var token = Assert.Single(tokens);
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void Lexes_TokenPairs(SyntaxKind firstKind, string firstText, SyntaxKind secondKind, string secondText)
    {
        void TestTokenPair(string separator)
        {
            var text = firstText + separator + secondText;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(firstKind, tokens[0].Kind);
            Assert.Equal(firstText, tokens[0].Text);
            Assert.Equal(secondKind, tokens[1].Kind);
            Assert.Equal(secondText, tokens[1].Text);
        }

        // Test with the appropriate separators
        if (RequiresSeparator(firstKind, secondKind))
        {
            TestTokenPair(" ");
            TestTokenPair("\t");
        }
        else // Test without any separator
        {
            TestTokenPair("");
        }
    }

    public static IEnumerable<object[]> GetTokensData()
    {
        foreach (var token in GetTokens())
            yield return new object[] {token.kind, token.text};
    }

    public static IEnumerable<object[]> GetTokenPairsData()
    {
        foreach (var token in GetTokenPairs())
            yield return new object[] {token.firstKind, token.firstText, token.secondKind, token.secondText};
    }

    private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            // Literal Tokens
            yield return (SyntaxKind.NumberToken, "123");
            yield return (SyntaxKind.IdentifierToken, "a");
            yield return (SyntaxKind.IdentifierToken, "reday");
            yield return (SyntaxKind.IdentifierToken, "loves");
            yield return (SyntaxKind.IdentifierToken, "ashley");

            // Operator Tokens
            yield return (SyntaxKind.PlusToken, "+");
            yield return (SyntaxKind.MinusToken, "-");
            yield return (SyntaxKind.StarToken, "*");
            yield return (SyntaxKind.SlashToken, "/");
            yield return (SyntaxKind.EqualsEqualsToken, "==");
            yield return (SyntaxKind.NotEqualsToken, "!=");
            yield return (SyntaxKind.EqualsToken, "=");

            // Punctuation Tokens
            yield return (SyntaxKind.OpenParenthesisToken, "(");
            yield return (SyntaxKind.CloseParenthesisToken, ")");

            // Keyword Tokens
            yield return (SyntaxKind.TrueKeyword, "True");
            yield return (SyntaxKind.FalseKeyword, "False");
            yield return (SyntaxKind.NotKeyword, "not");
            yield return (SyntaxKind.IsKeyword, "is");
            yield return (SyntaxKind.AndKeyword, "and");
            yield return (SyntaxKind.OrKeyword, "or");

            // Whitespace Tokens
            // yield return (SyntaxKind.WhitespaceToken, " ");
            // yield return (SyntaxKind.WhitespaceToken, "   ");
            // yield return (SyntaxKind.WhitespaceToken, "\t");
            // yield return (SyntaxKind.WhitespaceToken, "\n");
            // yield return (SyntaxKind.WhitespaceToken, "\r");
            // yield return (SyntaxKind.WhitespaceToken, "\r\n");
            // yield return (SyntaxKind.WhitespaceToken, "\r\t");
        }

        private static bool RequiresSeparator(SyntaxKind firstKind, SyntaxKind secondKind)
        {
            switch (firstKind)
            {
                case SyntaxKind.IdentifierToken:
                    return true; // Identifiers generally need separation from anything that might follow

                case SyntaxKind.NumberToken:
                    return secondKind == SyntaxKind.NumberToken || secondKind == SyntaxKind.IdentifierToken;

                case SyntaxKind.NotKeyword:
                    return true; // "not" should be separated from anything that follows to prevent merging

                case SyntaxKind.EqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                    return secondKind == SyntaxKind.EqualsToken || secondKind == SyntaxKind.EqualsEqualsToken;

                default:
                    // Check if the token kind is a keyword
                    return IsKeyword(firstKind);
            }
        }

        private static bool IsKeyword(SyntaxKind kind)
        {
            // Use a set to identify if a token kind is a keyword
            return Enum.GetName(typeof(SyntaxKind), kind)?.EndsWith("Keyword") ?? false;
        }

    private static IEnumerable<(SyntaxKind firstKind, string firstText, SyntaxKind secondKind, string secondText)> GetTokenPairs()
    {
        // Retrieves all tokens once and reuses the list to avoid multiple enumerations
        var allTokens = GetTokens().ToList();

        foreach (var firstToken in allTokens)
        {
            foreach (var secondToken in allTokens)
            {
                if (!RequiresSeparator(firstToken.kind, secondToken.kind))
                    yield return (firstToken.kind, firstToken.text, secondToken.kind, secondToken.text);
            }
        }
    }
}