using System.Collections;
using Compiler.Parts.Syntax;
using Compiler.Parts.Text;

namespace Compiler.Parts
{
    // Stores diagnostic messages that occur during the compilation process
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        // List to store all the Diagnostics collected
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        // Allows us to iterate over the diagnostics
        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Adds diagnostics from another Diagnostic Bag
        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        // Creates a new Diagnostic and adds it to the current bag
        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        // Invalid number: if the number provided isn't the right type
        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"The number {text} isn't a valid {type}.";
            Report(span, message);
        }

        // Bad character: If the character provided isn't recognized
        public void ReportBadCharacter(int position, char character)
        {
            var span = new TextSpan(position, 1);
            var message = $"Bad character in input: '{character}'.";
            Report(span, message);
        }

        // Unexpected token: If there is a token that's not where it should be
        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(span, message);
        }

        // Undefined unary operator: If we have a unrecognizable or out of place unary operator
        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type: {operandType}.";
            Report(span, message);
        }

        // Undefined binary operator: If we have a unrecognizable or out of place binary operator
        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type boundLeftType, Type boundRightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types: {boundLeftType} and {boundRightType}.";
            Report(span, message);
        }

        // Undefined name: If we have a name that hasn't been declared yet
        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable '{name}' does not exist.";
            Report(span, message);
        }
    }
}