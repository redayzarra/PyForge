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
            : this(null, syntaxTree)
        {}

        private Compilation(Compilation? previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public Compilation? Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        private BoundGlobalScope? _globalScope;

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        // Evaluate: expression provided by the syntaxTree tree 
        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            // Check for any error messages that have been collected, stops evaluation
            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);

            // Use the bound expression for evaluation and compute the value of expression
            var evaluator = new Evaluator(GlobalScope.Statement, variables);
            var value = evaluator.Evaluate();

            // Return the computed value and empty diagnostics (we checked)
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}