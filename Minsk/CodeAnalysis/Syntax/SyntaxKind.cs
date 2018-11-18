namespace Minsk.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        IdentifierToken,
        EqualsEqualsToken,
        EqualsToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        BangEqualsToken,
        LessToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,

        // Statements
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ExpressionStatement,

        // Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,

        // Keywords
        IfKeyword,
        ElseKeyword,
        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,

        // Nodes
        CompilationUnit,
        ElseClause,
    }
}
