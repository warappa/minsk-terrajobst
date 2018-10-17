using System;

namespace Minsk.CodeAnalysis
{
    class Evaluator
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
            if (root is NumberExpressionSyntax n)
            {
                return (int)n.NumberToken.Value;
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
