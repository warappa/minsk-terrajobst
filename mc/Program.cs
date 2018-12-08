using Minsk.CodeAnalysis.Binding;

namespace Minsk
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var repl = new MinskRepl();
            repl.Run();
        }
    }
}
