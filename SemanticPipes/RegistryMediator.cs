using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class RegistryMediator
    {
        private readonly HistoricalSemanticRegistryObserver _historicalSemanticRegistry =
            new HistoricalSemanticRegistryObserver();

        private readonly List<ISemanticRegistryObserver> _observers = new List<ISemanticRegistryObserver>();

        public RegistryMediator(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers = observers.ToList();
            _observers.Add(_historicalSemanticRegistry);
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _observers.Add(observer);

            IEnumerable<PipeOutputPackage> otherPacakgesToInstall =
                _historicalSemanticRegistry.NotifyObserverOfHistoricalRegistrations(observer);
            DoPackageInstallations(otherPacakgesToInstall);
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            IEnumerable<PipeOutputPackage> packagesToInsert = new[] {package};
            DoPackageInstallations(packagesToInsert);
        }

        private void DoPackageInstallations(IEnumerable<PipeOutputPackage> packagesToInstall)
        {
            IEnumerable<IEnumerable<PipeOutputPackage>> listOfAdditionalPackages =
                from installingPackages in packagesToInstall
                from observer in _observers
                let morePackages = observer.PipePackageInstalled(installingPackages)
                where morePackages != null
                select morePackages;

            foreach (var additionalPackagesToInstall in listOfAdditionalPackages)
            {
                DoPackageInstallations(additionalPackagesToInstall);
            }
        }

        private sealed class HistoricalSemanticRegistryObserver : ISemanticRegistryObserver
        {
            private readonly List<PipeOutputPackage> _pipePackages = new List<PipeOutputPackage>();

            public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
            {
                _pipePackages.Add(package);
                return null;
            }

            public IEnumerable<PipeOutputPackage> NotifyObserverOfHistoricalRegistrations(
                ISemanticRegistryObserver observer)
            {
                return
                    (from historicalPipePackage in _pipePackages
                        let morePackages = observer.PipePackageInstalled(historicalPipePackage)
                        where morePackages != null
                        select morePackages).SelectMany(x => x.ToArray());
            }
        }
    }
}