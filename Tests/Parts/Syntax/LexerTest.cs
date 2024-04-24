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

    public static IEnumerable<object[]> GetTokensData()
    {
        foreach (var token in GetTokens())
            yield return new object[] {token.kind, token.text};
    }

    private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            // Literal Tokens
            yield return (SyntaxKind.NumberToken, "123");
            yield return (SyntaxKind.IdentifierToken, "a");
            yield return (SyntaxKind.IdentifierToken, "reday");
            yield return (SyntaxKind.IdentifierToken, "zarra");

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
            yield return (SyntaxKind.AndKeyword, "and");
            yield return (SyntaxKind.OrKeyword, "or");
        }
}