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

