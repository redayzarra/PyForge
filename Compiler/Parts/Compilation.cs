using System.Collections.Immutable;
using Compiler.Parts.Binding;
using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    // Manages the compilation process: parsing and calculating results
    public class Compilation
    {
        // Initialize the compilation with the syntaxTree tree
        public Compilation(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        // Evaluate: expression provided by the syntaxTree tree 
        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            // Binder with dictionary of variables -> links them to expressions
            var binder = new Binder(variables);
            // Root of syntaxTree tree is processed into an expression, suitable format for evaluation
            var boundExpression = binder.BindExpression(SyntaxTree.Root.Expression);

            // Check for any error messages that have been collected, stops evaluation
            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);

            // Use the bound expression for evaluation and compute the value of expression
            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();

            // Return the computed value and empty diagnostics (we checked)
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}