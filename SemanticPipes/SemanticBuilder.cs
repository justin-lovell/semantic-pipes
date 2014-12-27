using System;
using System.Threading.Tasks;
using SemanticPipes.Observers;
using SemanticPipes.Registries;
using SemanticPipes.Solvers;

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
            return new Broker(SolverChainFactory.Create(graphEdges));
        }

        public SemanticBuilder InstallPipe<TSource, TDestination>(
            Func<TSource, ISemanticBroker, TDestination> processCallback)
        {
            if (processCallback == null)
            {
                throw new ArgumentNullException("processCallback");
            }

            PipeOutputPackage package = CreatePipeOutputPackage(processCallback);
            _registryMediator.AppendPackage(package);

            return this;
        }

        public SemanticBuilder InstallPipe<TSource, TDestination>(
            Func<TSource, ISemanticBroker, Task<TDestination>> processCallback)
        {
            if (processCallback == null)
            {
                throw new ArgumentNullException("processCallback");
            }

            PipeOutputPackage package = CreateAsyncPipeOutputPackage(processCallback);
            _registryMediator.AppendPackage(package);

            return this;
        }

        private PipeOutputPackage CreateAsyncPipeOutputPackage<TSource, TDestination>(
            Func<TSource, ISemanticBroker, Task<TDestination>> processCallback)
        {
            PipeCallback wrappedProcessCallback = (input, broker) =>
            {
                var castedInput = (TSource) input;

                return
                    processCallback(castedInput, broker)
                        .ContinueWith(task => (object) task.Result);
            };

            return PipeOutputPackage.Direct(typeof (TSource), typeof (TDestination), wrappedProcessCallback);
        }

        private static PipeOutputPackage CreatePipeOutputPackage<TSource, TDestination>(
            Func<TSource, ISemanticBroker, TDestination> processCallback)
        {
            PipeCallback wrappedProcessCallback = (input, broker) =>
            {
                var castedInput = (TSource) input;
                object result = processCallback(castedInput, broker);
                return result.IntoTaskResult();
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