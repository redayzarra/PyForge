using System.Collections;

namespace Compiler.Parts
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"The number {text} isn't a valid {type}";
            Report(span, message);
        }

        internal void ReportBadCharacter(int position, char character)
        {
            var span = new TextSpan(position, 1);
            var message = $"ERROR: Bad character in input: '{character}'";
            Report(span, message);
        }
    }
}