namespace SemanticPipes
{
    internal static class SolverChainFactory
    {
        public static ISolver Create(ISolver solver)
        {
            solver = new SimplifyEnumerableTypeResolving(solver);

            return solver;
        }
    }
}