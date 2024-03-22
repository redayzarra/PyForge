﻿using System;
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
                Console.WriteLine("Please enter an expression (or press Enter to exit):");
                Console.WriteLine();
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                var parser = new Parser(line);
                var expression = parser.Parse();

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(expression);
                Console.WriteLine();
                Console.ForegroundColor = color;
            }
        }

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

        public Lexer(string text)
        {
            _text = text;
        }

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
                var start = _position;
                
                while (char.IsDigit(Current))
                    Next();
                
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (int.TryParse(text, out var value))
                {
                    return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
                }
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
            Next();
            return new SyntaxToken(SyntaxKind.BadToken, _position - 1, _text.Substring(_position - 1, 1), null);
        } 
    } 

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {

    }

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

    class Parser 
    {
        private readonly SyntaxToken[] _tokens;
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
        }

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

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public ExpressionSyntax Parse()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.PlusToken || 
                   Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }
}

