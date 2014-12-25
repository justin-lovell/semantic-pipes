using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class RegistryMediator
    {
        private readonly HistoricalSemanticRegistryObserver _historicalSemanticRegistry =
            new HistoricalSemanticRegistryObserver();

        private readonly ShortestPathRegistryObserver _shortestPathObserver = new ShortestPathRegistryObserver();

        private readonly List<ISemanticRegistryObserver> _observers = new List<ISemanticRegistryObserver>();

        private readonly SafetyTripGuard _safetyTrip = new SafetyTripGuard();

        public RegistryMediator(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers.AddRange(observers);

            // todo: extract this out as a separate class, not observer???
            _observers.Add(_historicalSemanticRegistry);
            // todo: extract it out as an outside observer
            _observers.Add(_shortestPathObserver);
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _safetyTrip.DoAction(() =>
            {
                _observers.Add(observer);

                IEnumerable<PipeOutputPackage> otherPacakgesToInstall =
                    _historicalSemanticRegistry.NotifyObserverOfHistoricalRegistrations(observer);
                DoPackageInstallations(otherPacakgesToInstall);
            });
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            _safetyTrip.DoAction(() =>
            {
                IEnumerable<PipeOutputPackage> packagesToInsert = new[] { package };
                DoPackageInstallations(packagesToInsert);
            });
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

        public ISolver CreateSolver()
        {
            // todo: extract this class out
            return _shortestPathObserver;
        }
    }
}