using System.Collections.Immutable;

namespace Compiler.Parts.Text
{
    public class SourceText
    {
        private SourceText(string text)
        {
            Lines = ParseLines(this, text);
        }

        public ImmutableArray<TextLine> Lines { get; }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();
            var position = 0;
            var lineStart = 0;

            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);
                
                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(sourceText, position, lineStart, lineBreakWidth, result);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position > lineStart)
                AddLine(sourceText, position, lineStart, 0, result); // Ensure this call also passes `result`

            return result.ToImmutable();
        }

        private static void AddLine(SourceText sourceText, int position, int lineStart, int lineBreakWidth, ImmutableArray<TextLine>.Builder result)
        {
            var lineLength = position - lineStart;
            var lineLengthWithBreak = lineLength + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthWithBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            if (position < 0 || position >= text.Length)
                throw new ArgumentOutOfRangeException(nameof(position), "Index is outside the bounds of the text string.");

            var character = text[position];
            var lookAhead = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (character == '\r' && lookAhead == '\n')
                return 2;
            
            if (character == '\r' || character == '\n')
                return 1;

            return 0;
        }

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }
    }

    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthWithLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthWithLineBreak = lengthWithLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
        public int LengthWithLineBreak { get; }
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanWithLineBreak => new TextSpan(Start, LengthWithLineBreak);
    }
}