using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private IEnumerator<SyntaxNode> enumerator;
        private bool hasErrors = false;

        public AssertingEnumerator(SyntaxNode node)
        {
            enumerator = Flatten(node).GetEnumerator();
        }

        private bool MarkFailed()
        {
            hasErrors = true;
            return false;
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                yield return n;

                foreach (var child in n.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(kind, enumerator.Current.Kind);
                Assert.IsNotType<SyntaxToken>(enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertToken(SyntaxKind kind, string text)
        {
            try
            {
                Assert.True(enumerator.MoveNext());

                Assert.Equal(kind, enumerator.Current.Kind);
                var token = Assert.IsType<SyntaxToken>(enumerator.Current);

                Assert.Equal(text, token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void Dispose()
        {
            if (!hasErrors)
            {
                Assert.False(enumerator.MoveNext());
            }

            enumerator.Dispose();
        }
    }
}
