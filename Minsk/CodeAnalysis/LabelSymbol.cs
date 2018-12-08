namespace Minsk.CodeAnalysis
{
    public sealed class LabelSymbol
    {
        internal LabelSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
