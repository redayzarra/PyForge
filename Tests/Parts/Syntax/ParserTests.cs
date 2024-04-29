using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public partial class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBinaryOperatorPairs))]
    public void BinaryExpression_Precedence(SyntaxKind firstOperator, SyntaxKind secondOperator)
    {
        var firstPrecedence = SyntaxFacts.GetBinaryOperator(firstOperator);
        var secondPrecedence = SyntaxFacts.GetBinaryOperator(secondOperator);
        var firstText = SyntaxFacts.GetText(firstOperator);
        var secondText = SyntaxFacts.GetText(secondOperator);

        var text = $"a {firstText} b {secondText} c";

        if (firstPrecedence >= secondPrecedence)
        {

        }
        else 
        {
            
        }
    }

    public static IEnumerable<object[]> GetBinaryOperatorPairs()
    {
        var operators = SyntaxFacts.GetBinaryOperatorKinds().ToList();
        return from firstOperator in operators
               from secondOperator in operators
               select new object[] { firstOperator, secondOperator };
    }

}
