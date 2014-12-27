using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes.Registries
{
    internal sealed class ObserverRegistryMediator : IRegistryMediator
    {
        private readonly List<ISemanticRegistryObserver> _observers = new List<ISemanticRegistryObserver>();

        public ObserverRegistryMediator(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers.AddRange(observers);
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            foreach (var semanticRegistryObserver in _observers)
            {
                var siblingPackageLateBounded = semanticRegistryObserver.SiblingPackageLateBounded(observer);
                DoPackageInstallations(siblingPackageLateBounded);
            }

            _observers.Add(observer);
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            IEnumerable<PipeOutputPackage> packagesToInsert = new[] {package};
            DoPackageInstallations(packagesToInsert);
        }

        private void DoPackageInstallations(IEnumerable<PipeOutputPackage> packagesToInstall)
        {
            if (packagesToInstall == null)
            {
                return;
            }

            IEnumerable<IEnumerable<PipeOutputPackage>> listOfAdditionalPackages =
                from installingPackages in packagesToInstall
                where installingPackages != null
                from observer in _observers
                let morePackages = observer.PipePackageInstalled(installingPackages)
                select morePackages;

            foreach (var additionalPackagesToInstall in listOfAdditionalPackages)
            {
                DoPackageInstallations(additionalPackagesToInstall);
            }
        }
    }
}