using System;
using System.Collections.Generic;

namespace SemanticPipes.Solvers
{
    internal static class SolverChainFactory
    {
        public static ISolver Create(IEnumerable<KeyValuePair<Tuple<Type, Type>, PipeOutputPackage>> graphEdges)
        {
            ISolver solver = new GraphEdgedSolver(graphEdges);
            solver = new SimplifyEnumerableTypeResolving(solver);

            return solver;
        }
    }
}