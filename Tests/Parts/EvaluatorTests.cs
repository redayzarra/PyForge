using Compiler.Parts;
using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts;

public partial class ParserTests
{
    public class EvaluationTests
    {
        [Theory]
        // Testing basic arithmetic operations
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1 + 2", 3)]
        [InlineData("8 - 2", 6)]
        [InlineData("8 * 2", 16)]
        [InlineData("12 / 4", 3)]
        [InlineData("1 + 2 * 3", 7)]
        [InlineData("(1 + 2) * 3", 9)]

        // Testing booleans
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
        
        // Testing variables
        [InlineData("x = 10", 10)]
        [InlineData("(x = 4) + 2", 6)]
        [InlineData("(x = 10) * 2", 20)]  
        [InlineData("(x = 10) * x", 100)]
        [InlineData("a = (x = 12)", 12)]
        [InlineData("(a = 12) == 12", true)]
        [InlineData("(a = 12) != 12", false)]
        [InlineData("(a = 12) is 12", false)]
        [InlineData("(a = 12) is not 12", true)]

        // Testing scoping
        [InlineData("{x = 20}", 20)]
        [InlineData("{x = (x = 12)}", 12)]
        [InlineData("{x = True}", true)]
        [InlineData("{x = False}", false)]
        [InlineData("{x = True and False}", false)]
        public void EvaluateText(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}

