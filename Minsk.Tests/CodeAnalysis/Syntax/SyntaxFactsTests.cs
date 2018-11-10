using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactsTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText_RoundTrip(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);
            if (text == null)
            {
                return;
            }

            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);

            Assert.Equal(kind, token.Kind);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));

            foreach (var kind in kinds)
            {
                yield return new object[] { kind };
            }
        }
    }
}
