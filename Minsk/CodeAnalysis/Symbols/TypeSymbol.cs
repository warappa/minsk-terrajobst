namespace Minsk.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?");
        public static readonly TypeSymbol Void = new TypeSymbol("void");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol Int = new TypeSymbol("int");
        public static readonly TypeSymbol String = new TypeSymbol("string");

        internal TypeSymbol(string name)
            : base(name)
        {
        }

        public override SymbolKind Kind => SymbolKind.Type;
    }
}
