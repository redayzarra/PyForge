using Compiler.Parts.Syntax;
using Xunit.Abstractions;

namespace Compiler.Tests.Parts.Syntax;

public class LexerTests
{
    // Cache all tokens and separators for use in tests.
    private static readonly List<(SyntaxKind kind, string text)> AllTokens = GetTokens().ToList();
    private static readonly List<(SyntaxKind kind, string text)> AllSeparators = GetSeparators().ToList();

    private readonly ITestOutputHelper _output;

    public LexerTests(ITestOutputHelper output)
    {
        _output = output;
        // dotnet test --logger "trx;LogFileName=test_results.xml"
    }

    [Fact]
    public void LexesAllTokensCorrectly()
    {
        // Retrieve all SyntaxKinds that are either keywords or tokens
        var relevantKinds = Enum.GetValues(typeof(SyntaxKind))
                                .Cast<SyntaxKind>()
                                .Select(kind => new { Kind = kind, Name = kind.ToString() })
                                .Where(item => item.Name.EndsWith("Keyword") || item.Name.EndsWith("Token"))
                                .Select(item => item.Kind);

        // Get kinds already tested by combining tokens and separators
        var testedKinds = GetTokens().Concat(GetSeparators()).Select(t => t.kind);

        // Create a sorted set of relevant kinds and remove exceptions
        var untestedKinds = new SortedSet<SyntaxKind>(relevantKinds);
        untestedKinds.Remove(SyntaxKind.BadToken);
        untestedKinds.Remove(SyntaxKind.EndOfFileToken);
        untestedKinds.Remove(SyntaxKind.IsNotKeyword);

        // Remove all tested kinds from the set of relevant kinds
        untestedKinds.ExceptWith(testedKinds);

        // Assert that there are no untested kinds left
        Assert.Empty(untestedKinds);
    }
    
    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexes_Token(SyntaxKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text);
        var token = Assert.Single(tokens);

        _output.WriteLine($"Expected kind: {kind}, Actual kind: {token.Kind}");
        _output.WriteLine($"Expected text: '{text}', Actual text: '{token.Text}'");

        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    // Tests lexing of token pairs to ensure they are parsed correctly without separators
    [Theory]
    [MemberData(nameof(GetTokenPairs))]
    public void Lexes_TokenPairs(SyntaxKind firstKind, string firstText, SyntaxKind secondKind, string secondText)
    {
        void TestTokenPair(string separator)
        {
            _output.WriteLine($"Testing pair: '{firstText}{separator}{secondText}'");
            var text = firstText + separator + secondText;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();
            Assert.Equal(2, tokens.Length);
            Assert.Equal(firstKind, tokens[0].Kind);
            Assert.Equal(firstText, tokens[0].Text);
            Assert.Equal(secondKind, tokens[1].Kind);
            Assert.Equal(secondText, tokens[1].Text);
        }

        // Insert a separator if required by the syntax rules.
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

    // Tests lexing of token pairs with separators to ensure correct parsing with explicit separators.
    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparators))]
    public void Lexes_TokenPairs_WithSeparators(SyntaxKind firstKind, string firstText,
                                                SyntaxKind separatorKind, string separatorText,
                                                SyntaxKind secondKind, string secondText)
    {
        _output.WriteLine($"Testing pair with explicit separator: '{firstText}{separatorText}{secondText}'");
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

    // Provides data for token lexing tests.
    public static IEnumerable<object[]> GetTokensData() => AllTokens.Select(t => new object[] { t.kind, t.text });

    // Generates pairs of tokens that do not require separators.
    public static IEnumerable<object[]> GetTokenPairs() =>
        from first in AllTokens
        from second in AllTokens
        where !RequiresSeparator(first.kind, second.kind)
        select new object[] { first.kind, first.text, second.kind, second.text };

    // Generates pairs of tokens that require separators.
    public static IEnumerable<object[]> GetTokenPairsWithSeparators() =>
        from first in AllTokens
        from second in AllTokens
        where RequiresSeparator(first.kind, second.kind)
        from sep in AllSeparators
        select new object[] { first.kind, first.text, sep.kind, sep.text, second.kind, second.text };

    // Defines all tokens
    private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
    {
        // Obtain all fixed tokens and handle null text cases by converting them to empty strings
        var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
                            .Cast<SyntaxKind>()
                            .Select(k => (kind: k, text: SyntaxFacts.GetText(k)))
                            .Where(t => t.text != null && t.text != "is not")  // Continue excluding 'is not'
                            .Select(t => (t.kind, text: t.text ?? string.Empty));

        var dynamicTokens = new[]
        {
            // Explicitly specifying non-null string literals
            (SyntaxKind.NumberToken, "123"),
            (SyntaxKind.IdentifierToken, "a"),
            (SyntaxKind.IdentifierToken, "reday"),
            (SyntaxKind.IdentifierToken, "loves"),
            (SyntaxKind.IdentifierToken, "ashley"),
        };

        // Concatenate fixed and dynamic tokens ensuring all have non-null strings
        return fixedTokens.Concat(dynamicTokens);
    }

    // Defines separators
    private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
    {
        // Whitespace Tokens: spaces, tabs, and new lines.
        yield return (SyntaxKind.WhitespaceToken, " ");
        yield return (SyntaxKind.WhitespaceToken, "   ");
        yield return (SyntaxKind.WhitespaceToken, "\t");
        yield return (SyntaxKind.WhitespaceToken, "\n");
        yield return (SyntaxKind.WhitespaceToken, "\r");
        yield return (SyntaxKind.WhitespaceToken, "\r\n");
        yield return (SyntaxKind.WhitespaceToken, "\r\t");
    }

    // Determines if a separator is required between two tokens.
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
            case SyntaxKind.GreaterThanToken:
            case SyntaxKind.LessThanToken:
            case SyntaxKind.PlusToken:         // Include PlusToken
            case SyntaxKind.MinusToken:        // Include MinusToken
            case SyntaxKind.StarToken:
            case SyntaxKind.SlashToken:
            case SyntaxKind.PlusEqualsToken:   // Include PlusEqualsToken
            case SyntaxKind.MinusEqualsToken:  // Include MinusEqualsToken
            case SyntaxKind.StarEqualsToken:  // Include MinusEqualsToken
            case SyntaxKind.SlashEqualsToken:  // Include MinusEqualsToken
                return secondKind == SyntaxKind.EqualsToken || secondKind == SyntaxKind.EqualsEqualsToken ||
                    secondKind == SyntaxKind.PlusEqualsToken || secondKind == SyntaxKind.MinusEqualsToken;

            default:
                return IsKeyword(firstKind);
        }
    }

    // Checks if a token is a keyword: my naming convention is "Keyword"
    private static bool IsKeyword(SyntaxKind kind)
    {
        // Determine if a token kind is a keyword based on naming convention
        return Enum.GetName(typeof(SyntaxKind), kind)?.EndsWith("Keyword") ?? false;
    }
}
