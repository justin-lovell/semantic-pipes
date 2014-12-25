using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal class ReplayPackagesToLateBoundedObserverRegistryObserver : ISemanticRegistryObserver
    {
        private readonly List<PipeOutputPackage> _historicalPackages = new List<PipeOutputPackage>();
 
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            _historicalPackages.Add(package);
            return null;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            var additionalPackages =
                from historicalPackage in _historicalPackages
                let siblingPackages = siblingObserver.PipePackageInstalled(historicalPackage)
                where siblingPackages != null
                select siblingPackages;

            return additionalPackages.SelectMany(x => x);
        }
    }
}