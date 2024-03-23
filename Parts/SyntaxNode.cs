using System.Collections.Generic;

namespace Compiler.Parts
{
    // Defines our syntax node to build our syntax tree
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}