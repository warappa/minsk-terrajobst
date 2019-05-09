using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Minsk.CodeAnalysis.Symbols
{
    internal static class BuiltinFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol("rnd", ImmutableArray.Create<ParameterSymbol>(new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Int);

        internal static IEnumerable<FunctionSymbol> GetAll()
        {
            yield return Print;
            yield return Input;
            yield return Random;
        }
    }
}
