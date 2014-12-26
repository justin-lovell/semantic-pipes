using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes.Observers
{
    public sealed class CollectiveSemanticRegistryObserver : ISemanticRegistryObserver
    {
        private readonly IEnumerable<ISemanticRegistryObserver> _observers;

        public CollectiveSemanticRegistryObserver(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers = observers.ToArray();
        }

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            return
                from observer in _observers
                let newPackagesToInstall = observer.PipePackageInstalled(package)
                where newPackagesToInstall != null
                from additionalPackage in newPackagesToInstall
                select additionalPackage;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return
                from observer in _observers
                let newPackagesToInstall = observer.SiblingPackageLateBounded(siblingObserver)
                where newPackagesToInstall != null
                from additionalPackage in newPackagesToInstall
                select additionalPackage;
        }
    }
}