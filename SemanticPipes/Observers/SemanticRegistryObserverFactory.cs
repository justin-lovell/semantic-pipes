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
//            yield return new AdvertiveContravarianceChainObserver();

            // convert single objects to enumerables
            yield return new ConvertSingleOutputToEnumerableObserver();
            yield return new ConvertEnumerableToEnumerableObserver();

            // convert enumerables to target complex types
            yield return new ConvertEnumerableToTargetObserver(new ArrayEnumberableConversionStrategy());
            yield return new ConvertEnumerableToTargetObserver(new ListEnumberableConversionStrategy());

            // convert complex list type to basic enumerable
            yield return new ConvertEnumerableToTargetObserver(new IntoEnumberableConversionStrategy());
        }
    }
}