namespace Compiler.Parts.Syntax
{
    // Defines our syntax node to build our syntax tree
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}