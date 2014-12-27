using System.Collections.Generic;
using System.Linq;
using SemanticPipes.Observers;

namespace SemanticPipes.Registries
{
    internal static class RegistryMediatorFactory
    {
        public static IRegistryMediator Create(IEnumerable<ISemanticRegistryObserver> semanticRegistryObservers,
            ShortestPathGraphBuilder graphBuilder)
        {
// ReSharper disable once JoinDeclarationAndInitializer
            IRegistryMediator mediator;

            semanticRegistryObservers =
                semanticRegistryObservers.Concat(new ISemanticRegistryObserver[]
                {
                    new ShortestPathGraphBuilderAdapterRegistryObserver(graphBuilder)
                });

            mediator = new ObserverRegistryMediator(semanticRegistryObservers);
            mediator = new SimplifyPipePackageOutput(mediator);
            mediator = new SafetyRegistryMediator(mediator);

            return mediator;
        }
    }
}