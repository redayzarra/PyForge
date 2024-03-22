using System;
using System.Collections.Generic;
using System.Linq;

namespace rz
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                // Input: Extracts the input from console in 'line'
                Console.WriteLine();
                Console.Write("Enter an expression: ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                // Parse the current line from the console
                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                // Console styling
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine();

                // Pretty print the synax tree from the parser
                PrettyPrint(syntaxTree.Root);
                Console.WriteLine();
                Console.ForegroundColor = color;

                // If we have no errors, then go ahead and evaluate tree
                if (!syntaxTree.Diagnostics.Any())
                {
                    var evaluator = new Evaluator(syntaxTree.Root);
                    var result = evaluator.Evaluate();

                    // Console styling
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"Result: {result}");
                    Console.WriteLine();
                    Console.ForegroundColor = color;
                }
                // If we have any diagnostics, list them all 
                else 
                {
                    // Console styling and printing diagnostic
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var diagnostic in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostic);
                    Console.WriteLine(); 
                    Console.ForegroundColor = color;
                }
            }
        }

        // Creates a really pretty tree similar to Unix tree (folders)
        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken token && token.Value != null)
            {
                Console.Write(" ");
                Console.Write(token.Value);
            }
            Console.WriteLine();

            indent += isLast ? "    " : "│   ";
            
            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }

    // Basically a list of all the things my compiler can recognize
    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression, 
        BinaryExpression,
    }

    // A specific token in the given text (syntax of programming language)
    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    // Analyzes text and breaks it down into syntax tokens for parsing
    class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => _position >= _text.Length ? '\0' : _text[_position];

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // Skip any whitespace
            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            // After skipping whitespaces, check if we're at the end of the text
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

            // If the char is a number, then grab the entire number and return it
            if (char.IsDigit(Current))
            {
                // From our current position, let's try extracting the num
                var start = _position;
                
                // Keep increasing the pointer to get the whole number
                while (char.IsDigit(Current))
                    Next();
                
                // Calculate the length of the text and the text itself
                var length = _position - start;
                var text = _text.Substring(start, length);

                // Error handling if the number can't be converted to int
                if (!int.TryParse(text, out var value))
                    _diagnostics.Add($"The number {_text} isn't a valid Int32");
            
                // Otherwise, it's valid and we are good to go
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            // Handling operators and parentheses
            var tokenKind = Current switch
            {
                '+' => SyntaxKind.PlusToken,
                '-' => SyntaxKind.MinusToken,
                '*' => SyntaxKind.StarToken,
                '/' => SyntaxKind.SlashToken,
                '(' => SyntaxKind.OpenParenthesisToken,
                ')' => SyntaxKind.CloseParenthesisToken,
                _ => SyntaxKind.BadToken,
            };

            // If we recognize the special token (operator), then return it
            if (tokenKind != SyntaxKind.BadToken)
            {
                // Consume the character for a recognized token
                var tokenText = _text.Substring(_position, 1);
                _position++;
                return new SyntaxToken(tokenKind, _position - 1, tokenText, null);
            }

            // Otherwise, skip the unrecognized character and return a bad token
            _diagnostics.Add($"ERROR: Bad character in input: '{Current}'");
            Next();
            return new SyntaxToken(SyntaxKind.BadToken, _position - 1, _text.Substring(_position - 1, 1), null);
        } 
    } 

    // Defines our syntax node to build our syntax tree
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {

    }

    // Syntax node that holds numbers
    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    // Creates a binary expression (e.g. 4 + 5) and holds the operator
    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
    }

    // Uses the tokens from the Lexer to create a syntax tree
    class Parser 
    {
        private readonly SyntaxToken[] _tokens;
        private List<string> _diagnostics = new List<string>();
        private int _position;

        // Creates a list of tokens from a given text
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;

            do
            { // Add valid tokens to the _tokens array
                token = lexer.NextToken();
                if (token.Kind != SyntaxKind.WhitespaceToken && 
                    token.Kind != SyntaxKind.BadToken) 
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        // Given a certain offset, look at the token at that index
        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index < 0 || index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            
            return _tokens[index];
        }

        // Current is the token at the current position
        private SyntaxToken Current => Peek(0);

        // Returns the current token, then moves to next token
        private SyntaxToken NextToken()
        {
            // Return the same token we started with, but shift to next
            var current = Current;
            _position++;
            return current;
        }

        // If the token matches what it's suppose to be, move on or create null node
        private SyntaxToken Match(SyntaxKind kind)
        {
            // If the current token is the same kind, return the current
            if (Current.Kind == kind)
                return NextToken(); // But move the Parser's focus to next

            // Otherwise, return a placeholder token with null content
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        // Builds binary expressions for "+" and "-" operators
        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);

            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseTerm()
        {
            // Start with the first part of the expression (left side)
            var left = ParseFactor();

            // Keep going as long as we see "+" or "-" operators
            while (Current.Kind == SyntaxKind.PlusToken || 
                   Current.Kind == SyntaxKind.MinusToken)
            {
                // Capture the operator and combine the left and right sides
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            // Return the constructed expression (can be number or binary)
            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            // Start with the first part of the expression (left side)
            var left = ParsePrimaryExpression();

            // Keep going as long as we see "*" or "/" operators
            while (Current.Kind == SyntaxKind.StarToken ||
                   Current.Kind == SyntaxKind.SlashToken)
            {
                // Capture the operator and combine the left and right sides
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            // Return the constructed expression (can be number or binary)
            return left;
        }

        // Parses a primary expression (simplest form) into syntax node
        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }

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

