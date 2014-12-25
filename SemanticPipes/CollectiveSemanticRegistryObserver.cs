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
                _observers.SelectMany(semanticRegistryObserver => semanticRegistryObserver.PipePackageInstalled(package));
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return
                _observers.SelectMany(
                    semanticRegistryObserver => semanticRegistryObserver.SiblingPackageLateBounded(siblingObserver));
        }
    }
}