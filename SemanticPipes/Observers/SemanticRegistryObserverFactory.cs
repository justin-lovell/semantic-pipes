using System.Collections.Generic;

namespace SemanticPipes.Observers
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
            yield return new ConvertEnumerableToEnumerableObserver();
            yield return new ConvertEnumerableToTargetObserver(new ArrayEnumberableConversionStrategy());

            // todo: walkthrough inference chain (for list of generic type)
        }
    }
}