using Compiler.Parts;
using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

public partial class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBinaryOperatorPairs))]
    public void BinaryExpression_Precedence(SyntaxKind firstOperator, SyntaxKind secondOperator)
    {
        // Get the binary operator precendece for both operators
        var firstPrecedence = SyntaxFacts.GetBinaryOperator(firstOperator);
        var secondPrecedence = SyntaxFacts.GetBinaryOperator(secondOperator);

        // Get the text or "symbol" associated with the operator
        var firstText = SyntaxFacts.GetText(firstOperator) ?? "[Undefined Operator]";
        var secondText = SyntaxFacts.GetText(secondOperator) ?? "[Undefined Operator]";

        // Arrange operator text into expression and parse it using SyntaxTree
        var text = $"a {firstText} b {secondText} c";
        var expression = ParseExpression(text);

        using (var exp = new AssertingEnumerator(expression))
        {

            if (firstPrecedence >= secondPrecedence)
            {
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertToken(firstOperator, firstText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
                exp.AssertToken(secondOperator, secondText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "c");
            }
            else
            {
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertToken(firstOperator, firstText);
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
                exp.AssertToken(secondOperator, secondText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "c");
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorPairs))]
    public void UnaryExpression_Precedence(SyntaxKind unaryKind, SyntaxKind binaryKind)
    {
        // Get the unary and binary operator precendeces
        var unaryPrecedence = SyntaxFacts.GetUnaryOperator(unaryKind);
        var binaryPrecedence = SyntaxFacts.GetBinaryOperator(binaryKind);

        // Get the text or "symbol" associated with the operator
        var unaryText = SyntaxFacts.GetText(unaryKind) ?? "[Undefined Operator]";
        var binaryText = SyntaxFacts.GetText(binaryKind) ?? "[Undefined Operator]";

        // Arrange operator text into expression and parse it using SyntaxTree
        var text = $"{unaryText} a {binaryText} b";
        var expression = ParseExpression(text);

        using (var exp = new AssertingEnumerator(expression))
        {

            if (unaryPrecedence >= binaryPrecedence)
            {
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.UnaryExpression);
                exp.AssertToken(unaryKind, unaryText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertToken(binaryKind, binaryText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
            }
            else
            {
                exp.AssertNode(SyntaxKind.UnaryExpression);
                exp.AssertToken(unaryKind, unaryText);
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertToken(binaryKind, binaryText);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
            }
        }
    }

    private static ExpressionSyntax ParseExpression(string text)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var root = syntaxTree.Root;
        var statement = root.Statement;
        return Assert.IsType<ExpressionStatementSyntax>(statement).Expression;
    }

    public static IEnumerable<object[]> GetBinaryOperatorPairs()
    {
        foreach (var firstOperator in SyntaxFacts.GetBinaryOperatorKinds())
        {
            foreach (var secondOperator in SyntaxFacts.GetBinaryOperatorKinds())
            {
                yield return new object[] { firstOperator, secondOperator };
            }
        }
    }

    public static IEnumerable<object[]> GetUnaryOperatorPairs()
    {
        foreach (var unaryOperator in SyntaxFacts.GetUnaryOperatorKinds())
        {
            foreach (var binaryOperator in SyntaxFacts.GetBinaryOperatorKinds())
            {
                yield return new object[] { unaryOperator, binaryOperator };
            }
        }
    }
}
