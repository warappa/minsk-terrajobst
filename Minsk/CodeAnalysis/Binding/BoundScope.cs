using System.Collections.Generic;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables = null;
        private Dictionary<string, FunctionSymbol> functions = null;
        public BoundScope Parent { get; }

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (variables is null)
            {
                variables = new Dictionary<string, VariableSymbol>();
            }

            if (variables.ContainsKey(variable.Name))
            {
                return false;
            }

            variables.Add(variable.Name, variable);

            return true;
        }
        public bool TryDeclareFunction(FunctionSymbol function)
        {
            if (functions is null)
            {
                functions = new Dictionary<string, FunctionSymbol>();
            }

            if (functions.ContainsKey(function.Name))
            {
                return false;
            }

            functions.Add(function.Name, function);

            return true;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            variable = null;

            if (variables?.TryGetValue(name, out variable) == true)
            {
                return true;
            }

            if (Parent == null)
            {
                return false;
            }

            return Parent.TryLookupVariable(name, out variable);
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function)
        {
            function = null;

            if (functions?.TryGetValue(name, out function) == true)
            {
                return true;
            }

            if (Parent == null)
            {
                return false;
            }

            return Parent.TryLookupFunction(name, out function);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables?.Values.ToImmutableArray() ?? ImmutableArray<VariableSymbol>.Empty;
        }
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            return functions?.Values.ToImmutableArray() ?? ImmutableArray<FunctionSymbol>.Empty;
        }
    }
}
