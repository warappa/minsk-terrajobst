namespace Minsk.CodeAnalysis.Binding
{

    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;

        public LabelSymbol Label { get; }
    }

    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool jumpIfFalse = false)
        {
            Label = label;
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;

        public LabelSymbol Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfFalse { get; }
    }

    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }

        public LabelSymbol Label { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
    }
}
