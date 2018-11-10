using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;

            var marker = isLast ? "└── " : "├── ";

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }

            writer.Write(indent);
            writer.Write(marker);

            if (isToConsole)
            {
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Yellow : ConsoleColor.Cyan;
            }

            writer.Write(node.Kind);

            if (node is SyntaxToken st &&
                st.Value != null)
            {
                writer.Write(" ");
                writer.Write(st.Value);
            }

            writer.WriteLine();

            if (isToConsole)
            {
                Console.ResetColor();
            }

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
