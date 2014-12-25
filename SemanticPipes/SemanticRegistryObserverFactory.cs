using System.Collections.Generic;

namespace SemanticPipes
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateObservers()
        {
            yield return new ReplayPackagesToLateBoundedObserverRegistryObserver();
            yield return new EnsureNoDuplicateUserRegistrationObserver();


            var scaffolodingObservers = CreateScaffoldingObservers();
            var collectiveObserverForScaffolding = new CollectiveSemanticRegistryObserver(scaffolodingObservers);
            yield return new MemorisedChainBuilderRegistryBuilder(collectiveObserverForScaffolding);
        }

        private static IEnumerable<ISemanticRegistryObserver> CreateScaffoldingObservers()
        {
            yield return new AdvertiseOutputInheritanceChainObserver();
            yield return new ConvertSingleOutputToEnumerableObserver();

            // todo: convert to an array
            // todo: walkthrough inheritance chain
            // todo: walkthrough inference chain (for list of generic type)
        }
    }
}