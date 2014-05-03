using System;

namespace SemanticPipes
{
    public sealed class SemanticBroker : ISemanticBroker
    {
        private readonly SemanticPipelineSolver _solver = new SemanticPipelineSolver();

        public ISemanticOpenPipe On<TSource>(TSource source)
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");

            return new SolvingPipe<TSource>(_solver, source);
        }

        internal void AppendRegistry(ISemanticRegistry registry)
        {
            _solver.AppendRegistry(registry);
        }

        private class SolvingPipe<TSource> : ISemanticOpenPipe
        {
            private readonly SemanticPipelineSolver _solver;
            private readonly object _source;

            public SolvingPipe(SemanticPipelineSolver solver, object source)
            {
                _solver = solver;
                _source = source;
            }

            public TDestination Output<TDestination>()
            {
                PipeOutputPackage solvedPipePackage = _solver.SolveAsPipePackage(typeof (TSource), typeof (TDestination));
                object processedOutput = solvedPipePackage.ProcessInput(_source);

                return (TDestination) processedOutput;
            }
        }
    }
}