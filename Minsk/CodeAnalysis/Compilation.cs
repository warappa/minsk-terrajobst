using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Lowering;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
    public class Compilation
    {
        private BoundGlobalScope globalScope;

        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {
        }

        public Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    var newGlobalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref globalScope, newGlobalScope, null);
                }

                return globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            var statement = GetStatement();
            var evaluator = new Evaluator(statement, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }

        public void EmitTree(TextWriter @writer)
        {
            var statement = GetStatement();

            statement.WriteTo(writer);
        }

        private BoundBlockStatement GetStatement()
        {
            var result = GlobalScope.Statement;

            return Lowerer.Lower(result);
        }
    }
}
