using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
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
            mediator = new SafetyRegistryMediator(mediator);

            return mediator;
        }
    }
}