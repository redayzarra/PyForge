using Compiler.Parts;
using Compiler.Parts.Syntax;
using Compiler.Parts.Text;
using Xunit;

namespace Compiler.Tests.Parts;

public partial class ParserTests
{
    public class EvaluationTests
    {
        [Theory]
        // Arithmetic Tests
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1 + 2", 3)]
        [InlineData("8 - 2", 6)]
        [InlineData("8 * 2", 16)]
        [InlineData("12 / 4", 3)]
        [InlineData("1 + 2 * 3", 7)]
        [InlineData("(1 + 2) * 3", 9)]

        // Boolean Tests
        [InlineData("1 == 1", true)]
        [InlineData("10 != 10", false)]
        [InlineData("True", true)]
        [InlineData("False", false)]
        [InlineData("not False", true)]
        [InlineData("not True", false)]
        [InlineData("True or False and True", true)]
        [InlineData("True != False", true)]
        [InlineData("True == False", false)]
        [InlineData("True is False", false)]
        [InlineData("True is not False", true)]

        // Assignment Tests
        [InlineData("x = 10", 10)]
        [InlineData("(x = 4) + 2", 6)]
        [InlineData("(x = 10) * 2", 20)]
        [InlineData("(x = 10) * x", 100)]
        [InlineData("a = (x = 12)", 12)]
        [InlineData("(a = 12) == 12", true)]
        [InlineData("(a = 12) != 12", false)]
        [InlineData("(a = 12) is 12", false)]
        [InlineData("(a = 12) is not 12", true)]

        // Scoping Tests
        [InlineData("{x = 20}", 20)]
        [InlineData("{x = (x = 12)}", 12)]
        [InlineData("{x = True}", true)]
        [InlineData("{x = False}", false)]
        [InlineData("{x = True and False}", false)]
        public void EvaluateText(string text, object expectedValue)
        {
            var result = EvaluateExpression(text);
            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void VariableDeclarationReports()
        {
            var text = @"
            {
                x = 10
                y = 100
                {
                    x = 20
                }
                [z]
            }";

            var diagnostics = "Variable 'z' does not exist.";

            AssertDiagnostics(text, diagnostics);
        }

        private EvaluationResult EvaluateExpression(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();

            return compilation.Evaluate(variables);
        }

        private void AssertDiagnostics(string text, string expectedDiagnostics)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var result = EvaluateExpression(annotatedText.Text);

            var expectedDiagnosticLines = AnnotatedText.UnindentLines(expectedDiagnostics);

            Assert.Equal(expectedDiagnosticLines.Count, result.Diagnostics.Length);

            for (int i = 0; i < expectedDiagnosticLines.Count; i++)
            {
                var expectedMessage = expectedDiagnosticLines[i];
                var actualMessage = result.Diagnostics[i].Message;

                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;

                AssertTextSpanEqual(expectedSpan, actualSpan);
            }
        }

        private void AssertTextSpanEqual(TextSpan expected, TextSpan actual)
        {
            Assert.Equal(expected.Start, actual.Start);
            Assert.Equal(expected.End, actual.End);
        }
    }
}

