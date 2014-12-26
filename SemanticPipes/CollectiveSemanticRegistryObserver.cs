using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class CollectiveSemanticRegistryObserver : ISemanticRegistryObserver
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
                from additionalPackage in observer.PipePackageInstalled(package)
                select additionalPackage;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return
                from observer in _observers
                from additionalPackage in observer.SiblingPackageLateBounded(siblingObserver)
                select additionalPackage;
        }
    }
}