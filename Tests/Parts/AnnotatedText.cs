using System.Collections.Immutable;
using System.Text;
using Compiler.Parts.Text;

namespace Compiler.Tests.Parts
{
    public partial class ParserTests
    {
        internal sealed class AnnotatedText
        {
            public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                Spans = spans;
            }

            public string Text { get; }
            public ImmutableArray<TextSpan> Spans { get; }

            public static AnnotatedText Parse(string text)
            {
                if (text == null) throw new ArgumentNullException(nameof(text));

                text = Unindent(text);

                var textBuilder = new StringBuilder();
                var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
                var startStack = new Stack<int>();

                var position = 0;
                foreach (var character in text)
                {
                    switch (character)
                    {
                        case '[':
                            startStack.Push(position);
                            break;
                        case ']':
                            if (startStack.Count == 0)
                                throw new ArgumentException("Too many ']' in text.", nameof(text));

                            var start = startStack.Pop();
                            var span = TextSpan.FromBounds(start, position);
                            spanBuilder.Add(span);
                            break;
                        default:
                            textBuilder.Append(character);
                            position++;
                            break;
                    }
                }

                if (startStack.Count != 0)
                    throw new ArgumentException("Missing ']' in text.", nameof(text));

                return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
            }

            private static string Unindent(string text)
            {
                var lines = UnindentLines(text);

                return string.Join(Environment.NewLine, lines);
            }

            public static List<string> UnindentLines(string text)
            {
                if (text == null) throw new ArgumentNullException(nameof(text));

                var lines = new List<string>();
                using (var reader = new StringReader(text))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                var minIndent = lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Min(line => line.Length - line.TrimStart().Length);

                for (var i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Length > 0 && lines[i].Length >= minIndent)
                    {
                        lines[i] = lines[i].Substring(minIndent);
                    }
                }

                lines = lines
                    .SkipWhile(string.IsNullOrWhiteSpace)
                    .Reverse()
                    .SkipWhile(string.IsNullOrWhiteSpace)
                    .Reverse()
                    .ToList();

                return lines;
            }
        }
    }
}

