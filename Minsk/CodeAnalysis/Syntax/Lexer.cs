using System.Collections.Generic;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    internal class Lexer
    {
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        private readonly SourceText text;
        private int start;
        private int position;
        private SyntaxKind kind;
        private object value;
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

        public Lexer(SourceText text)
        {
            this.text = text;
        }

        public SyntaxToken Lex()
        {
            // <numbers>
            // + - * / ( )
            // <whitespace>
            start = position;
            kind = SyntaxKind.BadToken;
            this.value = null;

            switch (Current)
            {
                case '\0':
                    kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    kind = SyntaxKind.PlusToken;
                    position++;
                    break;
                case '-':
                    kind = SyntaxKind.MinusToken;
                    position++;
                    break;
                case '*':
                    kind = SyntaxKind.StarToken;
                    position++;
                    break;
                case '/':
                    kind = SyntaxKind.SlashToken;
                    position++;
                    break;
                case '(':
                    kind = SyntaxKind.OpenParenthesisToken;
                    position++;
                    break;
                case ')':
                    kind = SyntaxKind.CloseParenthesisToken;
                    position++;
                    break;
                case '{':
                    kind = SyntaxKind.OpenBraceToken;
                    position++;
                    break;
                case '}':
                    kind = SyntaxKind.CloseBraceToken;
                    position++;
                    break;
                case '&':
                    if (Lookahead == '&')
                    {
                        kind = SyntaxKind.AmpersandAmpersandToken;
                        position += 2;
                    }
                    break;
                case '|':
                    if (Lookahead == '|')
                    {
                        kind = SyntaxKind.PipePipeToken;
                        position += 2;
                    }
                    break;
                case '=':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.EqualsToken;
                    }
                    else
                    {
                        kind = SyntaxKind.EqualsEqualsToken;
                        position++;
                    }
                    break;
                case '!':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.BangToken;
                    }
                    else
                    {
                        kind = SyntaxKind.BangEqualsToken;
                        position++;
                    }
                    break;
                case '<':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.LessToken;
                    }
                    else
                    {
                        kind = SyntaxKind.LessOrEqualsToken;
                        position++;
                    }
                    break;
                case '>':
                    position++;
                    if (Current != '=')
                    {
                        kind = SyntaxKind.GreaterToken;
                    }
                    else
                    {
                        kind = SyntaxKind.GreaterOrEqualsToken;
                        position++;
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ReadNumberToken();
                    break;
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    ReadWhitespace();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhitespace();
                    }
                    else
                    {
                        this.diagnostics.ReportBadCharacter(position, Current);
                        position++;
                    }
                    break;
            }


            var t = SyntaxFacts.GetText(kind);
            var length = position - start;

            if (t == null)
            {
                t = text.ToString(start, length);
            }
            return new SyntaxToken(kind, start, t, value);
        }

        private void ReadWhitespace()
        {
            while (char.IsWhiteSpace(Current))
            {
                position++;
            }

            kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
            {
                position++;
            }

            var length = position - start;
            var t = text.ToString(start, length);
            if (!int.TryParse(t, out int value))
            {
                diagnostics.ReportInvalidNumber(new TextSpan(position, length), t, typeof(int));
            }

            this.value = value;
            kind = SyntaxKind.NumberToken;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
            {
                position++;
            }

            var length = position - start;
            var t = text.ToString(start, length);
            kind = SyntaxFacts.GetKeywordKind(t);

        }
    }
}
