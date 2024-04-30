using Compiler.Parts.Binding;

namespace Compiler.Parts
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        // Runs the EvaluateExpression function on the root of the tree
        public object Evaluate()
        {
            return EvaluateExpression(_root);
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

            switch (bin.Operate.Kind)
            {
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    // Check for division by zero before performing division
                    if ((int)right == 0)
                    {
                        throw new DivideByZeroException("Attempted to divide by zero.");
                    }
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorKind.Identity:
                    return ReferenceEquals(left, right);
                case BoundBinaryOperatorKind.NonIdentity:
                    return !ReferenceEquals(left, right);
                default:
                    throw new Exception($"Unexpected binary operator {bin.Operate}");
            }
        }
    }
}