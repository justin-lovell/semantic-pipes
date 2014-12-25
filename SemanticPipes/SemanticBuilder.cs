using System;
using System.Linq;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly IRegistryMediator _registryMediator;

        private readonly ShortestPathGraphBuilder _graphBuilder;

        public SemanticBuilder()
        {
            _graphBuilder = new ShortestPathGraphBuilder();

            var semanticRegistryObservers = SemanticRegistryObserverFactory.CreateObservers();
            _registryMediator = RegistryMediatorFactory.Create(semanticRegistryObservers, _graphBuilder);
        }

        public ISemanticBroker CreateBroker()
        {
            var graphEdges = _graphBuilder.FetchShortestPathsBySourceToDestination();
            var solver = new GraphEdgedSolver(graphEdges);

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