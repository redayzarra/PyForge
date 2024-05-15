using System.Reflection;
using Compiler.Parts.Text;

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
            var isConsole = writer == Console.Out;
            var marker = isLast ? "└──" : "├──";

            // Write the indent with the correct color
            if (isConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(indent);       
            writer.Write(marker);

            if (isConsole)
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Cyan : ConsoleColor.DarkBlue;

            writer.Write($"{node.Kind.ToString()}:");

            if (node is SyntaxToken token && token.Value != null)
            {
                writer.Write(" ");
                if (isConsole)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                
                writer.Write($"\"{token.Value}\"");
            }

            Console.ResetColor();

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
