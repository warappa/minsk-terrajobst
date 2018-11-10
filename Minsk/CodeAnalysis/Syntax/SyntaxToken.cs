using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override SyntaxKind Kind { get; }
        public override TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
