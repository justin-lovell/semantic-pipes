using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateObservers()
        {
            yield return new EnsureNoDuplicateUserRegistrationObserver();
            yield return new ReplayPackagesToLateBoundedObserverRegistryObserver();

            var scaffoldingObservers = CreateScaffoldingObservers();
            var collectiveObserverForScaffolding = new CollectiveSemanticRegistryObserver(scaffoldingObservers);
            yield return new MemorisedChainBuilderRegistryBuilder(collectiveObserverForScaffolding);
        }

        private static IEnumerable<ISemanticRegistryObserver> CreateScaffoldingObservers()
        {
            // inheritance traversal
            yield return new AdvertiseOutputInheritanceChainObserver();
            yield return new AdvertiseContravarianceChainObserver();

            // convert single objects to enumerables
            yield return new ConvertSingleOutputToEnumerableObserver();
            yield return new ConvertEnumerableToEnumerableObserver();
        }
    }
}