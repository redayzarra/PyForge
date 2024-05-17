using Compiler.Parts.Binding;

namespace Compiler.Parts
{
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
            _lastValue = 0; // Initialize _lastValue to a non-null value
        }

        // Runs the EvaluateExpression function on the root of the tree
        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            switch (statement)
            {
                case BoundBlockStatement block:
                    EvaluateBlockStatement(block);
                    break;
                case BoundExpressionStatement exp:
                    EvaluateExpressionStatement(exp);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected node type: '{statement.Kind}'");
            }
        }

        // Recursive function to evaluate the SyntaxTree
        private object EvaluateExpression(BoundExpression root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            return root switch
            {
                BoundUnaryExpression una => EvaluateUnaryExpression(una),
                BoundLiteralExpression num => num.Value ?? throw new InvalidOperationException("Null value encountered."),
                BoundBinaryExpression bin => EvaluateBinaryExpression(bin),
                BoundVariableExpression var => _variables.TryGetValue(var.Variable, out var value) ? value : throw new Exception($"Variable '{var.Variable}' not found."),
                BoundAssignmentExpression asn => EvaluateAssignmentExpression(asn),
                _ => throw new InvalidOperationException($"Unexpected node type: '{root.Kind}'")
            };
        }

        private void EvaluateBlockStatement(BoundBlockStatement block)
        {
            foreach (var statement in block.Statements)
                EvaluateStatement(statement);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement exp)
        {
            _lastValue = EvaluateExpression(exp.Expression);
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression asn)
        {
            var value = EvaluateExpression(asn.Expression);
            _variables[asn.Variable] = value;
            return value;
        }

        // Evaluates the left and right operands then applies the operation
        private object EvaluateUnaryExpression(BoundUnaryExpression una)
        {
            var operand = EvaluateExpression(una.Operand);

            return una.Operate.Kind switch
            {
                BoundUnaryOperatorKind.Negation => -(int)operand,
                BoundUnaryOperatorKind.Identity => (int)operand,
                BoundUnaryOperatorKind.LogicalNegation => !(bool)operand, // Correct handling for boolean negation
                _ => throw new InvalidOperationException($"Unexpected unary operator {una.Operate}")
            };
        }

        // Evaluates the left and right expressions then applies the operation
        private object EvaluateBinaryExpression(BoundBinaryExpression bin)
        {
            var left = EvaluateExpression(bin.Left);
            var right = EvaluateExpression(bin.Right);

            return bin.Operate.Kind switch
            {
                BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
                BoundBinaryOperatorKind.Division =>
                    (int)right == 0 ? throw new DivideByZeroException("Attempted to divide by zero.") : (int)left / (int)right,
                BoundBinaryOperatorKind.Addition => (int)left + (int)right,
                BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
                BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
                BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
                BoundBinaryOperatorKind.Equals => Equals(left, right),
                BoundBinaryOperatorKind.NotEquals => !Equals(left, right),
                BoundBinaryOperatorKind.Identity => ReferenceEquals(left, right),
                BoundBinaryOperatorKind.NonIdentity => !ReferenceEquals(left, right),
                _ => throw new InvalidOperationException($"Unexpected binary operator {bin.Operate}")
            };
        }
    }
}

