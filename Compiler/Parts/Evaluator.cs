using Compiler.Parts.Binding;
using System.Text;

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
            return FormatResult(_lastValue);
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
                case BoundIfStatement ifs:
                    EvaluateIfStatement(ifs);
                    break;
                case BoundWhileStatement whi:
                    EvaluateWhileStatement(whi);
                    break;
                case BoundForStatement fs:
                    EvaluateForStatement(fs);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected node type: '{statement.Kind}'");
            }
        }

        private void EvaluateForStatement(BoundForStatement forStatement)
        {
            var rangeArray = (int[])EvaluateExpression(forStatement.UpperBound);

            foreach (var value in rangeArray)
            {
                _variables[forStatement.Variable] = value;
                EvaluateStatement(forStatement.Body);
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement whi)
        {
            while ((bool)EvaluateExpression(whi.Condition))
                EvaluateStatement(whi.Body);
        }

        private void EvaluateIfStatement(BoundIfStatement ifs)
        {
            var condition = (bool)EvaluateExpression(ifs.Condition);
            if (condition)
                EvaluateStatement(ifs.ThenStatement);
            else
            {
                bool elifEvaluated = false;
                foreach (var elifClause in ifs.ElifClauses)
                {
                    var elifCondition = (bool)EvaluateExpression(elifClause.Condition);
                    if (elifCondition)
                    {
                        EvaluateStatement(elifClause.Statement);
                        elifEvaluated = true;
                        break;
                    }
                }

                if (!elifEvaluated && ifs.ElseStatement != null)
                    EvaluateStatement(ifs.ElseStatement);
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
                BoundRangeExpression rng => EvaluateRangeExpression(rng),
                _ => throw new InvalidOperationException($"Unexpected node type: '{root.Kind}'")
            };
        }

        private object EvaluateRangeExpression(BoundRangeExpression rng)
        {
            var lowerBound = (int)EvaluateExpression(rng.LowerBound);
            var upperBound = rng.UpperBound != null ? (int)EvaluateExpression(rng.UpperBound) : 0;
            var step = rng.Step != null ? (int)EvaluateExpression(rng.Step) : 1;

            if (rng.UpperBound != null)
            {
                if (step == 0)
                    throw new InvalidOperationException("Step cannot be zero.");

                var range = new List<int>();
                if (step > 0)
                {
                    for (int i = lowerBound; i < upperBound; i += step)
                        range.Add(i);
                }
                else
                {
                    for (int i = lowerBound; i > upperBound; i += step)
                        range.Add(i);
                }
                return range.ToArray();
            }
            else
            {
                return Enumerable.Range(0, lowerBound).ToArray();
            }
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
                BoundUnaryOperatorKind.LogicalNegation => !(bool)operand, 
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
                BoundBinaryOperatorKind.GreaterThan => (int)left > (int)right,
                BoundBinaryOperatorKind.GreaterThanOrEquals => (int)left >= (int)right,
                BoundBinaryOperatorKind.LessThan => (int)left < (int)right,
                BoundBinaryOperatorKind.LessThanOrEquals => (int)left <= (int)right,
                BoundBinaryOperatorKind.Identity => ReferenceEquals(left, right),
                BoundBinaryOperatorKind.NonIdentity => !ReferenceEquals(left, right),
                BoundBinaryOperatorKind.In => ((int[])right).Contains((int)left), 
                _ => throw new InvalidOperationException($"Unexpected binary operator {bin.Operate}")
            };
        }

        private object FormatResult(object result)
        {
            if (result is int[] array)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(array[i]);
                }
                sb.Append("]");
                return sb.ToString();
            }
            return result;
        }
    }
}

