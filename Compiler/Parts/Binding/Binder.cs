using System.Collections.Immutable;
using Compiler.Parts.Syntax;

namespace Compiler.Parts.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;

        public Binder(BoundScope? parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope);
            var boundStatement = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, variables, boundStatement);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope? previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope? parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var variable in previous.Variables)
                {
                    scope.TryDeclare(variable);
                }

                parent = scope;
            }

            return parent ?? new BoundScope(null);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindStatement(StatementSyntax syntax) =>
            syntax.Kind switch
            {
                SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
                SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
                _ => throw new InvalidOperationException($"Unexpected syntax: {syntax.Kind}")
            };

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            // Create a new scope for the block
            var previousScope = _scope;
            _scope = new BoundScope(previousScope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            // Revert to the parent scope
            _scope = previousScope;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax) =>
            syntax.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
                SyntaxKind.NameExpression => BindNameExpression((NameExpressionSyntax)syntax),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
                _ => throw new InvalidOperationException($"Unexpected syntax: {syntax.Kind}")
            };

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, typeof(bool));
            var thenStatement = BindStatement(syntax.ThenStatement);
            
            var elifClauses = ImmutableArray.CreateBuilder<BoundElifClause>();
            foreach (var elifClause in syntax.ElifClauses)
            {
                var elifCondition = BindExpression(elifClause.Condition, typeof(bool));
                var elifStatement = BindStatement(elifClause.Statement);
                elifClauses.Add(new BoundElifClause(elifCondition, elifStatement));
            }
            
            BoundStatement? elseStatement = null;
            if (syntax.ElseClause != null)
            {
                elseStatement = BindStatement(syntax.ElseClause.ElseStatement);
            }

            return new BoundIfStatement(condition, thenStatement, elifClauses.ToImmutable(), elseStatement);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, Type targetType)
        {
            var result = BindExpression(syntax);
            if (result.Type != targetType)
                _diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);
            
            return result;
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) =>
            BindExpression(syntax.Expression);

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if (!_scope.TryLookup(name, out var variable) || variable == null)
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);
            VariableSymbol? variable;

            // Lookup the variable in the current scope only
            if (!_scope.TryLookupInCurrentScope(name, out _))
            {
                // Declare a new variable if it does not exist in the current scope
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryDeclare(variable);
            }
            else
            {
                // Update the existing variable's type to match the new expression type
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryUpdate(variable);
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax) =>
            new BoundLiteralExpression(syntax.Value);

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}
