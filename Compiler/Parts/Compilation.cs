using Compiler.Parts.Binding;
using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    // Manages the compilation process: parsing and calculating results
    public class Compilation
    {
        // Initialize the compilation with the syntax tree
        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }

        // Evaluate: expression provided by the syntax tree 
        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            // Binder with dictionary of variables -> links them to expressions
            var binder = new Binder(variables);
            // Root of syntax tree is processed into an expression, suitable format for evaluation
            var boundExpression = binder.BindExpression(Syntax.Root);

            // Check for any error messages that have been collected, stops evaluation
            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            // Use the bound expression for evaluation and compute the value of expression
            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();

            // Return the computed value and empty diagnostics (we checked)
            return new EvaluationResult(Array.Empty<Diagnostic>(), value);
        }
    }
}