using Compiler.Parts.Binding;
using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        // Runs the EvaluateExpression function on the root of the tree
        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        // Recursive function to evaluate the SyntaxTree
        private int EvaluateExpression(BoundExpression root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            return root switch
            {
                BoundUnaryExpression una => EvaluateUnaryExpression(una),
                BoundLiteralExpression num => (int)(num.Value ?? throw new InvalidOperationException("Null value encountered.")),
                BinaryExpressionSyntax bin => EvaluateBinaryExpression(bin),
                ParenthesizedExpressionSyntax paren => EvaluateExpression(paren.Expression),
                _ => throw new InvalidOperationException($"Unexpected node type: '{root.Kind}'")
            };
        }

        // Evaluates the left and right operands then applies the operation
        private int EvaluateUnaryExpression(BoundUnaryExpression una)
        {
            var operand = EvaluateExpression(una.Operand);
            return una.OperatorKind switch
            {
                BoundUnaryOperatorKind.Negation => -operand,
                BoundUnaryOperatorKind.Identity => operand,
                _ => throw new InvalidOperationException($"Unexpected unary operator {una.OperatorKind}")
            };
        }

        // Evaluates the left and right expressions then applies the operation
        private int EvaluateBinaryExpression(BinaryExpressionSyntax bin)
        {
            var left = EvaluateExpression(bin.Left);
            var right = EvaluateExpression(bin.Right);

            return bin.OperatorToken.Kind switch
            {
                SyntaxKind.PlusToken => left + right,
                SyntaxKind.MinusToken => left - right,
                SyntaxKind.StarToken => left * right,
                SyntaxKind.SlashToken when right == 0 => throw new InvalidOperationException("Division by zero."),
                SyntaxKind.SlashToken => left / right,
                _ => throw new InvalidOperationException($"Unexpected binary operator: '{bin.OperatorToken.Kind}'")
            };
        }
    }
}