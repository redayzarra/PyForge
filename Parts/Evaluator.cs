using Compiler.Parts.Binding;

namespace Compiler.Parts
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
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
                _ => throw new InvalidOperationException($"Unexpected node type: '{root.Kind}'")
            };
        }

        // Evaluates the left and right operands then applies the operation
        private int EvaluateUnaryExpression(BoundUnaryExpression una)
        {
            var operand = (int) EvaluateExpression(una.Operand);
            return una.OperatorKind switch
            {
                BoundUnaryOperatorKind.Negation => -operand,
                BoundUnaryOperatorKind.Identity => operand,
                _ => throw new InvalidOperationException($"Unexpected unary operator {una.OperatorKind}")
            };
        }

        // Evaluates the left and right expressions then applies the operation
        private int EvaluateBinaryExpression(BoundBinaryExpression bin)
        {
            var left = (int) EvaluateExpression(bin.Left);
            var right = (int) EvaluateExpression(bin.Right);

            return bin.OperatorKind switch
            {
                BoundBinaryOperatorKind.Addition => left + right,
                BoundBinaryOperatorKind.Subtraction => left - right,
                BoundBinaryOperatorKind.Multiplication => left * right,
                BoundBinaryOperatorKind.Division when right == 0 => throw new InvalidOperationException("Division by zero."),
                BoundBinaryOperatorKind.Division => left / right,
                _ => throw new InvalidOperationException($"Unexpected binary operator: '{bin.OperatorKind}'")
            };
        }
    }
}