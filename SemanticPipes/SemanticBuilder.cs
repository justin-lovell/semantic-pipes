using System;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly RegistryMediator _registryMediator =
            new RegistryMediator(SemanticRegistryObserverFactory.CreateInternalObservers());


        public ISemanticBroker CreateBroker()
        {
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

            var package = new PipeOutputPackage(1, typeof (TSource), typeof (TDestination), wrappedProcessCallback);
            return package;
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