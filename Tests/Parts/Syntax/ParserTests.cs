using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public partial class ParserTests
{
    [Theory]
    [MemberData(GetBinaryOperatorParis)]
    public void BinaryExpression_Precedence(SyntaxKind firstOperator, SyntaxKind secondOperator)
    {
        
    }

}
