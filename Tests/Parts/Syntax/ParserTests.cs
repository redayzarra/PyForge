using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

internal sealed class AssertingEnumerator : IDisposable
{
    private readonly IEnumerator<SyntaxNode> _enumerator;

    public AssertingEnumerator(SyntaxNode node)
    {
        _enumerator = Flatten(node).GetEnumerator();
    }

    private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
    {
        var stack = new Stack<SyntaxNode>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var n = stack.Pop();
            yield return n;

            foreach (var child in n.GetChildren().Reverse())
                stack.Push(child);
        }
    }

    public void AssertToken(SyntaxKind kind, string text)
    {
        Assert.True(_enumerator.MoveNext());
        var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    public void Dispose()
    {
        _enumerator.Dispose();
    }
}

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

        // Example check
        if (firstPrecedence >= secondPrecedence)
        {
            Assert.False(true);
        }
        else
        {
            Assert.False(true);
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

