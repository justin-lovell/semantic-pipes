using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class RegistryMediator : IRegistryMediator
    {
        private readonly ShortestPathRegistryObserver _shortestPathObserver = new ShortestPathRegistryObserver();

        private readonly List<ISemanticRegistryObserver> _observers = new List<ISemanticRegistryObserver>();


        public RegistryMediator(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers.AddRange(observers);
            // todo: extract it out as an outside observer
            _observers.Add(_shortestPathObserver);
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            foreach (var semanticRegistryObserver in _observers)
            {
                DoPackageInstallations(semanticRegistryObserver.SiblingPackageLateBounded(observer));
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

        public ISolver CreateSolver()
        {
            // todo: extract this class out
            return _shortestPathObserver;
        }
    }
}