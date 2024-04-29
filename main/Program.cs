using Compiler.Parts;
using Compiler.Parts.Syntax;

namespace Compiler
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to my compiler! Please type valid Python expressions.");
            Console.WriteLine();
            Console.ResetColor();
            var variables = new Dictionary<VariableSymbol, object>();

            var showTree = false;
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                // Switch for visibility features and command handling
                switch (line)
                {
                    case "#showTree":
                        showTree = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Showing parse tree.");
                        Console.ResetColor();
                        Console.WriteLine();
                        continue;
                    case "#hideTree":
                        showTree = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Hiding parse tree.");
                        Console.ResetColor();
                        Console.WriteLine();
                        continue;
                    case "#clear":
                        Console.Clear();
                        continue;
                    case "#rerun":
                        Environment.Exit(2);  // Exit code 2 to indicate a rerun request
                        return;
                    case "#test":
                        Environment.Exit(3);  // Exit code 2 to indicate a rerun request
                        return;
                    case "#exit":
                        Console.Clear();
                        return;
                }

                // Parse and evaluate the expression
                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);
                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    PrettyPrint(syntaxTree.Root);
                    Console.WriteLine();
                }

                // Display results or diagnostics
                if (!diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"Result: {result.Value}");
                    Console.ResetColor();
                }
                else
                {
                    DisplayDiagnostics(diagnostics, line);
                }
                Console.WriteLine();
            }
        }

        private static void DisplayDiagnostics(IEnumerable<Diagnostic> diagnostics, string line)
        {
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(diagnostic);
                Console.ResetColor();
                HighlightErrorInLine(line, diagnostic.Span);
            }
        }

        private static void HighlightErrorInLine(string line, TextSpan span)
        {
            var prefix = line.Substring(0, span.Start);
            var error = line.Substring(span.Start, span.Length);
            var suffix = line.Substring(span.End);

            Console.Write("    ");
            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ResetColor();
            Console.WriteLine(suffix);
            Console.WriteLine(new string(' ', span.Start + 4) + new string('^', span.Length));
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

