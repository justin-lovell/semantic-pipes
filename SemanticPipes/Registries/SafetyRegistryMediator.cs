namespace SemanticPipes.Registries
{
    internal class SafetyRegistryMediator : IRegistryMediator
    {
        private readonly SafetyTripGuard _safetyTrip = new SafetyTripGuard();
        private readonly IRegistryMediator _nextMediator;

        public SafetyRegistryMediator(IRegistryMediator nextMediator)
        {
            _nextMediator = nextMediator;
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _safetyTrip.DoAction(() => _nextMediator.AppendObserver(observer));
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            _safetyTrip.DoAction(() => _nextMediator.AppendPackage(package));
        }
    }
}