using System.Text;
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
            var textBuilder = new StringBuilder();
            Compilation? previous = null;

            while (true)
            {
                if (textBuilder.Length == 0)
                    PrintWithColor(">>> ", ConsoleColor.White, inline: true);
                else
                    PrintWithColor("└─ ", ConsoleColor.Gray, inline: true);

                Console.ForegroundColor = ConsoleColor.White; // Set input color to white
                var input = Console.ReadLine();
                Console.ResetColor(); // Reset color after input

                if (input == null)
                    break; // Exit if ReadLine() returns null (end of input stream)

                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                        break;

                    if (HandleCommand(input, ref showTree, ref previous, variables))
                        continue; // Skip parsing and evaluating if HandleCommand processed a command
                }

                textBuilder.AppendLine(input);

                var text = textBuilder.ToString();
                var syntaxTree = SyntaxTree.Parse(text);

                // Check for syntax errors but allow blank lines in the middle of input
                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = previous == null  
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);
                
                var result = compilation.Evaluate(variables);

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("│");
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                if (!result.Diagnostics.Any())
                {
                    Console.WriteLine();
                    PrintWithColor($"Result: ", ConsoleColor.Black, inline: true);
                    PrintWithColor($"{result.Value}", ConsoleColor.Magenta);

                    previous = compilation;
                }
                else
                    DisplayDiagnostics(result.Diagnostics, syntaxTree);

                textBuilder.Clear();
                Console.WriteLine();
            }
        }

        private static bool HandleCommand(string input, ref bool showTree, ref Compilation? previous, Dictionary<VariableSymbol, object> variables)
        {
            switch (input.Trim())
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
                case "clear()":
                    Welcome();
                    return true;
                case "reset()":
                    previous = null;
                    Welcome();
                    return true;
                case "run()":
                    PrintWithColor("Restarting compiler...", ConsoleColor.DarkGray);
                    Console.WriteLine();
                    Environment.Exit(2);
                    return true;
                case "test()":
                    PrintWithColor("Running tests...", ConsoleColor.DarkGray);
                    Console.WriteLine();
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

        private static void PrintWithColor(string message, ConsoleColor color, bool inline = false)
        {
            Console.ForegroundColor = color;
            if (inline)
                Console.Write(message);
            else 
                Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void DisplayDiagnostics(IEnumerable<Diagnostic> diagnostics, SyntaxTree syntaxTree)
        {
            foreach (var diagnostic in diagnostics)
            {
                var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                var line = syntaxTree.Text.Lines[lineIndex];
                var lineNumber = lineIndex + 1;
                var character = diagnostic.Span.Start - line.Start + 1;

                Console.WriteLine();
                PrintWithColor($"Line {lineNumber}, Char {character}: ", ConsoleColor.DarkRed, inline: true);
                PrintWithColor($"{diagnostic}", ConsoleColor.Yellow);

                HighlightErrorInLine(line.ToString(), diagnostic.Span, line.Start);
            }
        }

        private static void HighlightErrorInLine(string line, TextSpan span, int lineStart)
        {
            int startIndex = span.Start - lineStart;
            int length = span.Length;

            // Ensure startIndex and length are within the bounds of the line
            if (startIndex > line.Length)
                startIndex = line.Length;
            if (startIndex + length > line.Length)
                length = line.Length - startIndex;

            var prefix = line.Substring(0, startIndex);
            var error = line.Substring(startIndex, length);
            var suffix = line.Substring(startIndex + length);

            Console.WriteLine();
            PrintWithColor(">>> ", ConsoleColor.DarkGray, inline: true);
            PrintWithColor($"{prefix}", ConsoleColor.DarkGray, inline: true);

            PrintWithColor($"{error}", ConsoleColor.Red, inline: true);
            PrintWithColor($"{suffix}", ConsoleColor.DarkGray);

            if (length > 0)
            {
                Console.WriteLine(new string(' ', startIndex + 4) + new string('^', length));
            }
            else
            {
                Console.WriteLine(new string(' ', startIndex + 4) + "^");
            }
        }
    }
}


