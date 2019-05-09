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
        CallExpression,
        GotoStatement,
        ConditionalGotoStatement,
        LabelStatement,
        ExpressionStatement,

        // Expressions
        ErrorExpression,
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}
