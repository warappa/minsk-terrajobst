using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
    internal class Lexer
    {
        private List<string> diagnostics = new List<string>();
        private readonly string text;
        private int position;
        private char Current
        {
            get
            {
                if (position >= text.Length)
                {
                    return '\0';
                }

                return text[position];
            }
        }

        public IEnumerable<string> Diagnostics => diagnostics;

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

            if (position >= text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, position, "\0", null);
            }
            else if (char.IsDigit(Current))
            {
                var start = position;
                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = position - start;
                var t = text.Substring(start, length);

                if (!int.TryParse(t, out var value))
                {
                    diagnostics.Add($"The number {t} cannot be represented by an Int32.");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, t, value);
            }
            else if (char.IsWhiteSpace(Current))
            {
                var start = position;
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
                var start = position;
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
            }

            this.diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, position++, text.Substring(position - 1, 1), null);
        }
    }
}
