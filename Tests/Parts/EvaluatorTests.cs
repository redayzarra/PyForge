using Compiler.Parts;
using Compiler.Parts.Syntax;
using Compiler.Parts.Text;

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

        // Comparison Operator Tests
        [InlineData("10 > 5", true)]
        [InlineData("5 > 10", false)]
        [InlineData("10 >= 5", true)]
        [InlineData("5 >= 10", false)]
        [InlineData("10 >= 10", true)]
        [InlineData("5 < 10", true)]
        [InlineData("10 < 5", false)]
        [InlineData("5 <= 10", true)]
        [InlineData("10 <= 5", false)]
        [InlineData("10 <= 10", true)]

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

        // If Statements
        [InlineData("{a = 0 if a == 0: a = 10 a}", 10)]
        [InlineData("{a = 0 if a == 1: a = 10 else: a = 20 a}", 20)]
        [InlineData("{a = 10 if a == 9: hot = True elif a == 10: hot = False hot}", false)]
        [InlineData("{a = 0 if a == 1: a = 10 elif a == 0: a = 20 a}", 20)]
        [InlineData("{a = 0 if a == 1: a = 10 elif a == 2: a = 20 else: a = 30 a}", 30)]
        [InlineData("{a = 0 if a == 0: if a == 0: a = 10 else: a = 20 a}", 10)]
        [InlineData("{a = 3 if a == 1: a = 10 elif a == 2: a = 20 elif a == 3: a = 30 a}", 30)]
        [InlineData("{a = 2 if a == 1: a = 10 elif a == 2: a = 20 elif a == 3: a = 30 else: a = 40 a}", 20)]
        [InlineData("{a = 4 if a < 2: a = 10 elif a < 4: if a == 3: a = 30 else: a = 40 elif a == 4: a = 50 else: a = 60 a}", 50)]
        [InlineData("{a = 1 b = 2 if a + b == 3: a = 10 elif a - b == -1: a = 20 else: a = 30 a}", 10)]

        // While Statements
        [InlineData("{a = 10 res = 0 while a > 0: { res = res + a a = a - 1} res}", 55)]
        [InlineData("{a = 5 res = 1 while a > 1: { res = res * a a = a - 1} res}", 120)] // factorial of 5
        [InlineData("{a = 0 while a < 5: { a = a + 1 } a}", 5)]
        [InlineData("{a = 0 sum = 0 while a <= 5: { sum = sum + a a = a + 1 } sum}", 15)] // sum of first 5 numbers
        [InlineData("{a = 0 while False: { a = a + 1 } a}", 0)] // loop never executes
        [InlineData("{a = 0 b = 0 while a < 3: { a = a + 1 while b < 2: { b = b + 1 } } a + b}", 5)] // nested while loops

        // Range Tests
        [InlineData("range(10)", "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]")]
        [InlineData("range(5, 10)", "[5, 6, 7, 8, 9]")]
        [InlineData("range(10, -1, -1)", "[10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0]")]

        // In Keyword tests
        [InlineData("20 in range(30)", true)]
        [InlineData("10 in range(5, 15)", true)]
        [InlineData("5 in range(10, 0, -1)", true)]
        [InlineData("0 in range(1, 10)", false)]
        [InlineData("4 in range(1, 10, 2)", false)] // Tests with step that skips the value
        [InlineData("2 in range(10, 0, -2)", true)]

        // For Statements
        [InlineData("{sum = 0 for i in range(5): { sum = sum + i } sum}", 10)]
        [InlineData("{x = 10 for i in range(5): { x = x - 1 } x}", 5)]
        [InlineData("{sum = 0 for i in range(6): { sum = sum + i } sum}", 15)] // sum of first 5 numbers
        [InlineData("{res = 0 for i in range(3): { for j in range(2): { res = res + 1 } } res}", 6)] // nested for loops
        [InlineData("{sum = 0 for i in range(3): { sum = sum + i } sum}", 3)] // simple range iteration
        [InlineData("{res = 0 for i in range(3): { for j in range(2): { res = res + i * 10 + j } } res}", 63)] // nested loops with calculation
        [InlineData("{sum = 0 for i in range(5, 10): { sum = sum + i } sum}", 35)] // range with two arguments
        [InlineData("{sum = 0 for i in range(10, -1, -1): { sum = sum + i } sum}", 55)] // range with three arguments
        [InlineData("{x = 10 for i in range(10, 0, -2): { x = x - 1 } x}", 5)] // range with three arguments and negative step
        public void EvaluateText(string text, object expectedValue)
        {
            var result = EvaluateExpression(text);
            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        [Theory]
        [InlineData("{ [a] }", "Variable 'a' does not exist.")]
        [InlineData("{ x = 10 [y] }", "Variable 'y' does not exist.")]
        [InlineData("{ { x = 20 } [y] }", "Variable 'y' does not exist.")]
        [InlineData("{ a = 10 b = 20 c = 30 [d] }", "Variable 'd' does not exist.")]
        [InlineData("{ a = 10 b = 20 c = 30 d = 40 [e] }", "Variable 'e' does not exist.")]
        public void UndefinedVariable(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
        }

        [Theory]
        [InlineData("[+]True", "Unary operator '+' is not defined for type: System.Boolean.")]
        [InlineData("[-]False", "Unary operator '-' is not defined for type: System.Boolean.")]
        [InlineData("[not]1", "Unary operator 'not' is not defined for type: System.Int32.")]
        public void UndefinedUnaryOperator(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
        }

        [Theory]
        [InlineData("10 [*] False", "Binary operator '*' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [/] True", "Binary operator '/' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [+] True", "Binary operator '+' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [-] False", "Binary operator '-' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [==] True", "Binary operator '==' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [!=] False", "Binary operator '!=' is not defined for types: System.Int32 and System.Boolean.")]
        [InlineData("10 [and] 20", "Binary operator 'and' is not defined for types: System.Int32 and System.Int32.")]
        [InlineData("10 [or] 20", "Binary operator 'or' is not defined for types: System.Int32 and System.Int32.")]
        [InlineData("True [==] 10", "Binary operator '==' is not defined for types: System.Boolean and System.Int32.")]
        [InlineData("False [!=] 10", "Binary operator '!=' is not defined for types: System.Boolean and System.Int32.")]
        public void UndefinedBinaryOperator(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
        }

        [Theory]
        [InlineData("{a = 0 if [0]: a = 10 a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 if [1 + 2]: a = 10 a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 if [x = 5]: a = 10 a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 if [range(10)]: a = 10 a}", "Cannot convert type 'System.Int32[]' to 'System.Boolean'.")]
        public void IfStatementCannotConvert(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
        }

        [Theory]
        [InlineData("{a = 0 while [5]: { a = a + 1 } a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 while [1 + 2]: { a = a + 1 } a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 while [x = 5]: { a = a + 1 } a}", "Cannot convert type 'System.Int32' to 'System.Boolean'.")]
        [InlineData("{a = 0 while [range(10)]: { a = a + 1 } a}", "Cannot convert type 'System.Int32[]' to 'System.Boolean'.")]
        public void WhileStatementCannotConvert(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
        }

        [Theory]
        [InlineData("range([True], 10)", "Cannot convert type 'System.Boolean' to 'System.Int32'.")]
        [InlineData("range(100, [False])", "Cannot convert type 'System.Boolean' to 'System.Int32'.")]
        [InlineData("range(10, 20, [True])", "Cannot convert type 'System.Boolean' to 'System.Int32'.")]
        public void RangeExpressionCannotConvert(string text, string expectedDiagnostics)
        {
            AssertDiagnostics(text, expectedDiagnostics);
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

