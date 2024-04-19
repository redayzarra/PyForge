using Compiler.Parts;
using Compiler.Parts.Syntax;
using Compiler.Parts.Binding;

namespace Compiler
{
    internal static class Program
    {
        private static void Main()
        {
            var showTree = true;
            while (true)
            {
                // Input: Extracts the input from console in 'line'
                Console.WriteLine();
                Console.Write("Enter an expression: ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                // Setting up tree visibility feature
                switch (line)
                {
                    case "#showTree":
                        showTree = true;
                        break;
                    case "#hideTree":
                        showTree = false;
                        break;
                    default:
                        break;
                }

                // Making the console more convenient
                if (line == "#showTree" || line == "#hideTree")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(showTree ? "Showing parse tree." : "Hiding parse tree.");
                    Console.ResetColor();
                    continue;
                }
                else if (line == "clear" || line == "cleawr") {
                    Console.Clear();
                    continue;
                }
                else if (line == "exit")
                    break;
                

                // Parse the current line from the console
                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate();
                var diagnostics = result.Diagnostics;

                // Console styling
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine();

                if (showTree) {
                    // Pretty print the synax tree from the parser
                    PrettyPrint(syntaxTree.Root);
                    Console.WriteLine();
                    Console.ResetColor();
                }

                // If we have no errors, then go ahead and evaluate tree
                if (!diagnostics.Any())
                {
                    // Console styling
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"Result: {result.Value}");
                    Console.WriteLine();
                }
                // If we have any diagnostics, list them all 
                else 
                {
                    // Console styling and printing diagnostic
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var diagnostic in diagnostics)
                        Console.WriteLine(diagnostic);
                    Console.WriteLine(); 
                }
                Console.ResetColor();
            }
        }

        // Creates a really pretty tree similar to Unix tree (folders)
        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken token && token.Value != null)
            {
                Console.Write(" ");
                Console.Write(token.Value);
            }
            Console.WriteLine();

            indent += isLast ? "   " : "│  ";
            
            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }
}

