using Compiler.Parts.Text;

namespace Compiler.Tests.Parts.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]  
        [InlineData(".\r\n", 2)]
        [InlineData(".\n", 2)]
        [InlineData("Hello\nWorld", 2)]
        [InlineData("First line\nSecond line\nThird line", 3)] 
        [InlineData(".\r\n\r\n", 3)] 
        [InlineData("Line 1\nLine 2\nLine 3\nLine 4", 4)] 
        [InlineData(".\n\n\n", 4)] 
        public void SourceText_WithLastLine(string text, int expectedCount)
        {
            var sourceText = SourceText.From(text);
            Assert.Equal(expectedCount, sourceText.Lines.Length);
        }
    }
}

