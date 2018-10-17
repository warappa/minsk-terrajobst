using System;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression root;

        public Evaluator(BoundExpression root)
        {
            this.root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(root);
        }

        private object EvaluateExpression(BoundExpression root)
        {
            if (root is BoundLiteralExpression literal)
            {
                return literal.Value;
            }
            if (root is BoundUnaryExpression unary)
            {
                var operand = (int)EvaluateExpression(unary.Operand);

                switch (unary.OperatorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -operand;
                    default:
                        throw new Exception($"Unexpected unary operator {unary.OperatorKind}");
                }
            }

            if (root is BoundBinaryExpression binary)
            {
                var left = (int)EvaluateExpression(binary.Left);
                var right = (int)EvaluateExpression(binary.Right);

                switch (binary.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Substraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return left * right;
                    default:
                        throw new Exception($"Unexpected binary operator {binary.OperatorKind}");
                }
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}
