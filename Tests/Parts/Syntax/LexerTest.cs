using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public class LexerTest
{
    private static readonly List<(SyntaxKind kind, string text)> AllTokens = GetTokens().ToList();
    private static readonly List<(SyntaxKind kind, string text)> AllSeparators = GetSeparators().ToList();

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
    [MemberData(nameof(GetTokenPairs))]
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

        if (RequiresSeparator(firstKind, secondKind))
        {
            TestTokenPair(" ");
            TestTokenPair("\t");
        }
        else
        {
            TestTokenPair("");
        }
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparators))]
    public void Lexes_TokenPairs_WithSeparators(SyntaxKind firstKind, string firstText,
                                                SyntaxKind separatorKind, string separatorText,
                                                SyntaxKind secondKind, string secondText)
    {
        var text = firstText + separatorText + secondText;
        var tokens = SyntaxTree.ParseTokens(text).ToArray();
        Assert.Equal(3, tokens.Length);
        Assert.Equal(firstKind, tokens[0].Kind);
        Assert.Equal(firstText, tokens[0].Text);
        Assert.Equal(separatorKind, tokens[1].Kind);
        Assert.Equal(separatorText, tokens[1].Text);
        Assert.Equal(secondKind, tokens[2].Kind);
        Assert.Equal(secondText, tokens[2].Text);
    }

    public static IEnumerable<object[]> GetTokensData() => AllTokens.Select(t => new object[] { t.kind, t.text });

    public static IEnumerable<object[]> GetTokenPairs() =>
        from first in AllTokens
        from second in AllTokens
        where !RequiresSeparator(first.kind, second.kind)
        
        select new object[] { first.kind, first.text, second.kind, second.text };

    public static IEnumerable<object[]> GetTokenPairsWithSeparators() =>
        from first in AllTokens
        from second in AllTokens
        where RequiresSeparator(first.kind, second.kind)
        from sep in AllSeparators

        select new object[] { first.kind, first.text, sep.kind, sep.text, second.kind, second.text };

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
    }

    private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
    {
        // Whitespace Tokens
        yield return (SyntaxKind.WhitespaceToken, " ");
        yield return (SyntaxKind.WhitespaceToken, "   ");
        yield return (SyntaxKind.WhitespaceToken, "\t");
        yield return (SyntaxKind.WhitespaceToken, "\n");
        yield return (SyntaxKind.WhitespaceToken, "\r");
        yield return (SyntaxKind.WhitespaceToken, "\r\n");
        yield return (SyntaxKind.WhitespaceToken, "\r\t");
    }

    private static bool RequiresSeparator(SyntaxKind firstKind, SyntaxKind secondKind)
    {
        switch (firstKind)
        {
            case SyntaxKind.IdentifierToken:
            case SyntaxKind.NotKeyword:
                return true; // These kinds should be separated from anything that follows

            case SyntaxKind.NumberToken:
                return secondKind == SyntaxKind.NumberToken || secondKind == SyntaxKind.IdentifierToken;

            case SyntaxKind.EqualsToken:
            case SyntaxKind.EqualsEqualsToken:
                return secondKind == SyntaxKind.EqualsToken || secondKind == SyntaxKind.EqualsEqualsToken;

            default:
                return IsKeyword(firstKind);
        }
    }

    private static bool IsKeyword(SyntaxKind kind)
    {
        // Determine if a token kind is a keyword based on naming convention
        return Enum.GetName(typeof(SyntaxKind), kind)?.EndsWith("Keyword") ?? false;
    }
}
