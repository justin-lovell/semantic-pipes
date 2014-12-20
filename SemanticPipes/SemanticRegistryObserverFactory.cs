using System.Collections.Generic;

namespace SemanticPipes
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateInternalObservers()
        {
            //PipeInstalled += BuilderPipeAddOnForCollections.InstallHandler;
            yield break;
        }
    }
}