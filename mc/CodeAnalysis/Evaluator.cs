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
                var operand = EvaluateExpression(unary.Operand);

                switch (unary.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    default:
                        throw new Exception($"Unexpected unary operator {unary.Op}");
                }
            }

            if (root is BoundBinaryExpression binary)
            {
                var left = EvaluateExpression(binary.Left);
                var right = EvaluateExpression(binary.Right);

                switch (binary.Op.Kind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Substraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Division:
                        return (int)left / (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.LogicalAnd:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOr:
                        return (bool)left || (bool)right;
                    default:
                        throw new Exception($"Unexpected binary operator {binary.Op}");
                }
            }

            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}
