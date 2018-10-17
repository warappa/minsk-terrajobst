namespace Minsk.CodeAnalysis
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        Whitespace,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,

        // Expressions
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
