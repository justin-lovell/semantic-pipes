using System;

namespace SemanticPipes
{
    internal sealed class Broker : ISemanticBroker
    {
        private readonly Solver _solver;

        internal Broker(Solver solver)
        {
            _solver = solver;
        }

        public ISemanticOpenPipe On<TSource>(TSource source)
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");

            return new SolvingPipe<TSource>(_solver, source);
        }

        private class SolvingPipe<TSource> : ISemanticOpenPipe
        {
            private readonly Solver _solver;
            private readonly object _source;

            public SolvingPipe(Solver solver, object source)
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