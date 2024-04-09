using Compiler.Parts.Syntax;

namespace Compiler.Parts
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        // Runs the EvaluateExpression function on the root of the tree
        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        // Recursive function to evaluate the SyntaxTree
        private int EvaluateExpression(ExpressionSyntax root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            return root switch
            {
                UnaryExpressionSyntax una => EvaluateUnaryExpression(una),
                LiteralExpressionSyntax num => (int)(num.LiteralToken.Value ?? throw new InvalidOperationException("Null value encountered.")),
                BinaryExpressionSyntax bin => EvaluateBinaryExpression(bin),
                ParenthesizedExpressionSyntax paren => EvaluateExpression(paren.Expression),
                _ => throw new InvalidOperationException($"Unexpected node type: '{root.Kind}'")
            };
        }

        // Evaluates the left and right operands then applies the operation
        private int EvaluateUnaryExpression(UnaryExpressionSyntax una)
        {
            var operand = EvaluateExpression(una.Operand);
            return una.OperatorToken.Kind switch
            {
                SyntaxKind.MinusToken => -operand,
                SyntaxKind.PlusToken => operand,
                _ => throw new InvalidOperationException($"Unexpected unary operator {una.OperatorToken.Kind}")
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