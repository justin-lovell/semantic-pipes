using System.Collections.Generic;

namespace SemanticPipes
{
    internal static class SemanticRegistryObserverFactory
    {
        public static IEnumerable<ISemanticRegistryObserver> CreateObservers()
        {
            yield return new ReplayPackagesToLateBoundedObserverRegistryObserver();
            yield return new EnsureNoDuplicateUserRegistrationObserver();


            var scaffoldingObservers = CreateScaffoldingObservers();
            var collectiveObserverForScaffolding = new CollectiveSemanticRegistryObserver(scaffoldingObservers);
            yield return new MemorisedChainBuilderRegistryBuilder(collectiveObserverForScaffolding);
        }

        private static IEnumerable<ISemanticRegistryObserver> CreateScaffoldingObservers()
        {
            yield return new AdvertiseOutputInheritanceChainObserver();
            yield return new ConvertSingleOutputToEnumerableObserver();
            yield return new ConvertSingleOutputToArrayObserver();
            yield return new ConvertEnumerableToEnumerableObserver();

            // todo: walkthrough inference chain (for list of generic type)
        }
    }
}