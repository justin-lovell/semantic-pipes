using System.Collections.Generic;

namespace SemanticPipes
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateInternalObservers()
        {
            yield return new ReplayPackagesToLateBoundedObserverRegistryObserver();

            // todo: rename the follow classes
            yield return new BuilderPipeEnsureNoUserDuplication();
            yield return new BuilderPipeFromNonEnumerableToSingleItemList();
            // todo: convert to an array
            // todo: walkthrough inheritance chain
            // todo: walkthrough inference chain (for list of generic type)
        }
    }
}