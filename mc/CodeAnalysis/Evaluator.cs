using System;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
    public class Evaluator
    {
        private readonly ExpressionSyntax root;

        public Evaluator(ExpressionSyntax root)
        {
            this.root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(root);
        }

        private int EvaluateExpression(ExpressionSyntax root)
        {
            if (root is LiteralExpressionSyntax literal)
            {
                return (int)literal.LiteralToken.Value;
            }
            if (root is UnaryExpressionSyntax unary)
            {
                var operand = EvaluateExpression(unary.Operand);

                if (unary.OperatorToken.Kind == SyntaxKind.PlusToken)
                {
                    return operand;
                }
                if (unary.OperatorToken.Kind == SyntaxKind.MinusToken)
                {
                    return -operand;
                }
                else
                {
                    throw new Exception($"Unexpected unary operator {unary.OperatorToken.Kind}");
                }
            }

            if (root is BinaryExpressionSyntax binary)
            {
                var left = EvaluateExpression(binary.Left);
                var right = EvaluateExpression(binary.Right);

                if (binary.OperatorToken.Kind == SyntaxKind.PlusToken)
                {
                    return left + right;
                }
                if (binary.OperatorToken.Kind == SyntaxKind.MinusToken)
                {
                    return left - right;
                }
                if (binary.OperatorToken.Kind == SyntaxKind.SlashToken)
                {
                    return left / right;
                }
                if (binary.OperatorToken.Kind == SyntaxKind.StarToken)
                {
                    return left * right;
                }
                else
                {
                    throw new Exception($"Unexpected binary operator {binary.OperatorToken.Kind}");
                }
            }

            if (root is ParenthesizedExpressionSyntax parenthesized)
            {
                return EvaluateExpression(parenthesized.Expression);
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}
