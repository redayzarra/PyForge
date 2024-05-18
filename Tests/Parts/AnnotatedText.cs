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
                    if (character == '[')
                    {
                        startStack.Push(position);
                    }
                    else if (character == ']')
                    {
                        if (startStack.Count == 0)
                            throw new ArgumentException("Too many ']' in text.", nameof(text));

                        var start = startStack.Pop();
                        var end = position;
                        var span = TextSpan.FromBounds(start, end);
                        spanBuilder.Add(span);
                    }
                    else
                    {
                        textBuilder.Append(character);
                        position++;
                    }
                }

                if (startStack.Count != 0)
                    throw new ArgumentException("Missing ']' in text.", nameof(text));

                return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
            }

            private static string Unindent(string text)
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

                var minIndent = int.MaxValue;
                foreach (var line in lines)
                {
                    if (line.Trim().Length == 0)
                        continue;

                    var indent = line.Length - line.TrimStart().Length;
                    minIndent = Math.Min(minIndent, indent);
                }

                for (var i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Length == 0)
                        continue;

                    lines[i] = lines[i].Substring(minIndent);
                }

                while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
                    lines.RemoveAt(0);

                while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
                    lines.RemoveAt(lines.Count - 1);

                return string.Join(Environment.NewLine, lines);
            }
        }
    }
}

