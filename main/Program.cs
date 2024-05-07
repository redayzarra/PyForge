using Compiler.Parts;
using Compiler.Parts.Syntax;
using Compiler.Parts.Text;

namespace Compiler
{
    internal static class Program
    {
        private static void Main()
        {
            Welcome();
            var variables = new Dictionary<VariableSymbol, object>();
            var showTree = false;

            while (true)
            {
                Console.Write(">>> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (HandleCommand(line, ref showTree, variables))
                    continue; // Skip parsing and evaluating if HandleCommand processed a command

                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                if (showTree)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                    Console.WriteLine();
                }

                if (!result.Diagnostics.Any())
                    PrintWithColor($"{result.Value}", ConsoleColor.DarkGray);
                else
                    DisplayDiagnostics(result.Diagnostics, line);

                Console.WriteLine();
            }
        }

        private static bool HandleCommand(string line, ref bool showTree, Dictionary<VariableSymbol, object> variables)
        {
            switch (line)
            {
                case "showTree()":
                    showTree = true;
                    PrintWithColor("Showing parse tree.", ConsoleColor.DarkGreen);
                    Console.WriteLine();
                    return true;
                case "hideTree()":
                    showTree = false;
                    PrintWithColor("Hiding parse tree.", ConsoleColor.DarkGreen);
                    Console.WriteLine();
                    return true;
                case "cls":
                    variables.Clear(); // Clear variables - depends if I want to
                    Welcome();
                    return true;
                case "run()":
                    Environment.Exit(2);
                    return true;
                case "test()":
                    Environment.Exit(3);
                    return true;
                case "exit()":
                    Console.Clear();
                    Environment.Exit(0);
                    return true;
                default:
                    return false;
            }
        }

        private static void Welcome()
        {
            Console.Clear();
            PrintWithColor("Welcome to my compiler! Please type valid Python expressions.", ConsoleColor.Green);
            Console.WriteLine();
        }

        private static void PrintWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void DisplayDiagnostics(IEnumerable<Diagnostic> diagnostics, string line)
        {
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine();
                PrintWithColor(diagnostic.ToString(), ConsoleColor.DarkRed);
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
            Console.Write(suffix);
            Console.WriteLine();
            Console.WriteLine(new string(' ', span.Start + 4) + new string('^', span.Length));
        }
    }
}

