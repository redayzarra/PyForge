using System.Reflection;

namespace Compiler.Parts.Syntax
{
    // Defines our syntax node to build our syntax tree
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = property.GetValue(this) as SyntaxNode;
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = property.GetValue(this) as IEnumerable<SyntaxNode>;
                    if (children != null)
                    {
                        foreach (var child in children)
                            if (child != null)  // Additional null check
                                yield return child;
                    }
                }
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken token && token.Value != null)
            {
                writer.Write(": '");
                writer.Write(token.Value);
                writer.Write("'");
            }
            writer.WriteLine();

            indent += isLast ? "   " : "│  ";
            
            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}

