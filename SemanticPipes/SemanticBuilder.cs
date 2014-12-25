using System;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        // todo: extract this chain builder out.
        private readonly IRegistryMediator _registryMediator =
            new SafetyRegistryMediator(new RegistryMediator(SemanticRegistryObserverFactory.CreateInternalObservers()));


        public ISemanticBroker CreateBroker()
        {
            // todo: factor out the Solver Creator
            var solver = _registryMediator.CreateSolver();

            return new Broker(solver);
        }

        public SemanticBuilder InstallPipe<TSource, TDestination>(Func<TSource, TDestination> processCallback)
        {
            if (processCallback == null)
            {
                throw new ArgumentNullException("processCallback");
            }

            PipeOutputPackage package = CreatePipeOutputPackage(processCallback);
            _registryMediator.AppendPackage(package);

            return this;
        }

        private static PipeOutputPackage CreatePipeOutputPackage<TSource, TDestination>(
            Func<TSource, TDestination> processCallback)
        {
            Func<object, object> wrappedProcessCallback = rawInput =>
            {
                var castedInput = (TSource) rawInput;
                return processCallback(castedInput);
            };

            return PipeOutputPackage.Direct(typeof (TSource), typeof (TDestination), wrappedProcessCallback);
        }

        public void RegisterObserver(ISemanticRegistryObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            _registryMediator.AppendObserver(observer);
        }
    }
}