using System.Collections.Generic;

namespace SemanticPipes
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateInternalObservers()
        {
            yield return new BuilderPipeFromSingleObjectToSingleItemList();
            // todo: convert to an array
            // todo: walkthrough inheritance chain
        }
    }
}