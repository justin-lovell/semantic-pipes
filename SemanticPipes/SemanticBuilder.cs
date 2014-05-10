using System;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly Solver _currentSolver = new Solver();

        public SemanticBuilder Install(IPipeExtension pipeExtension)
        {
            _currentSolver.Install(pipeExtension);
            return this;
        }

        public SemanticBroker CreateBroker()
        {
            return new SemanticBroker(_currentSolver);
        }

        public SemanticBuilder Pipe<TSource, TDestination>(Func<TSource, TDestination> processCallback)
        {
            var extension = new GenericPipeExtension<TSource, TDestination>(processCallback);

            _currentSolver.Install(extension);

            return this;
        }
    }
}