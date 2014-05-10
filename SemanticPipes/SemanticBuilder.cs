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
    }
}