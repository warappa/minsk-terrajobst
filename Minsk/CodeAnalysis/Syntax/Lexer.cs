using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
    internal class Lexer
    {
        private DiagnosticBag diagnostics = new DiagnosticBag();
        private readonly string text;
        private int position;
        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset = 0)
        {
            var index = position + offset;
            if (index >= text.Length)
            {
                return '\0';
            }

            return text[index];
        }

        public DiagnosticBag Diagnostics => diagnostics;

        public Lexer(string text)
        {
            this.text = text;
        }

        public void Next()
        {
            position++;
        }

        public SyntaxToken Lex()
        {
            // <numbers>
            // + - * / ( )
            // <whitespace>
            var start = position;

            if (position >= text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, position, "\0", null);
            }
            else if (char.IsDigit(Current))
            {

                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = position - start;
                var t = text.Substring(start, length);

                if (!int.TryParse(t, out var value))
                {
                    diagnostics.ReportInvalidNumber(new TextSpan(position, length), t, typeof(int));
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, t, value);
            }
            else if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = position - start;
                var t = text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.Whitespace, start, t, null);
            }

            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                {
                    Next();
                }

                var length = position - start;
                var t = text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(t);
                return new SyntaxToken(kind, start, t, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, position++, ")", null);
                case '&':
                    if (Lookahead == '&')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                    }
                    break;
                case '|':
                    if (Lookahead == '|')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                    }
                    break;
                case '=':
                    if (Lookahead == '=')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                    else
                    {
                        return new SyntaxToken(SyntaxKind.EqualsToken, position++, "=", null);
                    }
                case '!':
                    if (Lookahead == '=')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                    }
                    else
                    {
                        position++;
                        return new SyntaxToken(SyntaxKind.BangToken, start, "!", null);
                    }
            }

            this.diagnostics.ReportBadCharacter(position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, position++, text.Substring(position - 1, 1), null);
        }
    }
}
