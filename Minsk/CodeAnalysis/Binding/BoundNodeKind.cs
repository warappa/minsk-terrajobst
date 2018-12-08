namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
        GotoStatement,
        ConditionalGotoStatement,
        LabelStatement,
        ExpressionStatement,

        // Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}
