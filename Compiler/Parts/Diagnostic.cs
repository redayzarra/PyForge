namespace Compiler.Parts
{
    // Diagnostics or error messages 
    public sealed class Diagnostic
    {
        // TextSpan (span) is where the error occurred, message is the warning explanation
        public Diagnostic(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }

        public TextSpan Span { get; }
        public string Message { get; }

        public override string ToString() => Message;
    }
}