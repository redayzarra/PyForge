using Compiler.Parts.Syntax;

namespace Compiler.Tests.Parts.Syntax;

internal sealed class AssertingEnumerator : IDisposable
{
    private readonly IEnumerator<SyntaxNode> _enumerator;
    private bool _hasErrors;

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
            var currentNode = stack.Pop();
            yield return currentNode;

            // Push the child nodes in reverse order to process them in original order
            foreach (var child in currentNode.GetChildren().Reverse())
                stack.Push(child);
        }
    }

    public void AssertToken(SyntaxKind kind, string text)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }

    public void AssertNode(SyntaxKind kind)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            var node = _enumerator.Current;
            Assert.IsNotType<SyntaxToken>(node);
            Assert.Equal(kind, node.Kind);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }

    private bool MarkFailed()
    {
        _hasErrors = true;
        return false;
    }

    public void Dispose()
    {
        if (!_hasErrors)
            Assert.False(_enumerator.MoveNext());
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
        var text = $"a {SyntaxFacts.GetText(firstOperator)} b {SyntaxFacts.GetText(secondOperator)} c";
        var expression = SyntaxTree.Parse(text).Root;

        using (var exp = new AssertingEnumerator(expression))
        {
            exp.AssertNode(SyntaxKind.BinaryExpression);

            if (firstPrecedence >= secondPrecedence)
            {
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "c");
            }
            else
            {
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "a");
                exp.AssertNode(SyntaxKind.BinaryExpression);
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "b");
                exp.AssertNode(SyntaxKind.NameExpression);
                exp.AssertToken(SyntaxKind.IdentifierToken, "c");
            }
        }
    }

    public static IEnumerable<object[]> GetBinaryOperatorPairs()
    {
        foreach (var firstOperator in SyntaxFacts.GetBinaryOperatorKinds())
        {
            foreach (var secondOperator in SyntaxFacts.GetBinaryOperatorKinds())
            {
                yield return new object[] { firstOperator, secondOperator };
                yield break;
            }
        }
    }
}

