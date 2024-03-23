using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parts
{
    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax root)
        {
            // Switch expression to handle different types of nodes
            return root switch
            {
                NumberExpressionSyntax num => (int)num.NumberToken.Value,
                BinaryExpressionSyntax bin => EvaluateBinaryExpression(bin),
                ParenthesizedExpressionSyntax paren => EvaluateExpression(paren.Expression),
                _ => throw new Exception($"Unexpected node type: '{root.Kind}'")
            };
        }

        private int EvaluateBinaryExpression(BinaryExpressionSyntax bin)
        {
            var left = EvaluateExpression(bin.Left);
            var right = EvaluateExpression(bin.Right);

            // Handling operations with a switch statement
            return bin.OperatorToken.Kind switch
            {
                SyntaxKind.PlusToken => left + right,
                SyntaxKind.MinusToken => left - right,
                SyntaxKind.StarToken => left * right,
                SyntaxKind.SlashToken => right != 0 ? left / right : throw new Exception("Division by zero."),
                _ => throw new Exception($"Unexpected binary operator: '{bin.OperatorToken.Kind}'")
            };
        }
    }
}