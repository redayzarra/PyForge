using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public partial class ParserTests
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKind))]
        public void GetTextRoundTrip(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);
            if (text == null) return; // Skip testing kinds with no associated text

            var tokens = SyntaxTree.ParseTokens(text)
                                   .Where(t => t.Kind != SyntaxKind.WhitespaceToken)
                                   .ToList(); // Filter out whitespace tokens once and for all

            if (kind == SyntaxKind.IsNotKeyword)
            {
                // Assert that we only have the "is" and "not" tokens
                Assert.Equal(2, tokens.Count); // Ensure there are exactly two tokens
                Assert.Equal(SyntaxKind.IsKeyword, tokens[0].Kind);
                Assert.Equal("is", tokens[0].Text);
                Assert.Equal(SyntaxKind.NotKeyword, tokens[1].Kind);
                Assert.Equal("not", tokens[1].Text);
            }
            else
            {
                // Ensure there is exactly one token that matches the kind and text
                var token = Assert.Single(tokens);
                Assert.Equal(kind, token.Kind);
                Assert.Equal(text, token.Text);
            }
        }

        public static IEnumerable<object[]> GetSyntaxKind()
        {
            return Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>()
                       .Select(kind => new object[] { kind });
        }
    }
}

