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
            Assert.Equal(kind, _enumerator.Current.Kind);
            var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
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
            Assert.Equal(kind, node.Kind);
            Assert.IsNotType<SyntaxToken>(node);
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
        // Get the binary operator precendece for both operators
        var firstPrecedence = SyntaxFacts.GetBinaryOperator(firstOperator);
        var secondPrecedence = SyntaxFacts.GetBinaryOperator(secondOperator);

        // Get the text or "symbol" associated with the operator
        var firstText = SyntaxFacts.GetText(firstOperator) ?? "[Undefined Operator]";
        var secondText = SyntaxFacts.GetText(secondOperator) ?? "[Undefined Operator]";

        // Arrange operator text into expression and parse it using SyntaxTree
        var text = $"a {firstText} b {secondText} c";
        var expression = SyntaxTree.Parse(text).Root;

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
}

