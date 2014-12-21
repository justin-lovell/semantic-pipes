﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class RegistryMediator
    {
        private readonly HistoricalSemanticRegistryObserver _historicalSemanticRegistry =
            new HistoricalSemanticRegistryObserver();

        private readonly KillSwitchObserver _killSwitchObserver = new KillSwitchObserver();

        private readonly ShortestPathRegistryObserver _shortestPathObserver = new ShortestPathRegistryObserver();

        private readonly List<ISemanticRegistryObserver> _observers = new List<ISemanticRegistryObserver>();

        public RegistryMediator(IEnumerable<ISemanticRegistryObserver> observers)
        {
            _observers.Add(_killSwitchObserver);
            _observers.AddRange(observers);
            _observers.Add(_historicalSemanticRegistry);
            _observers.Add(_shortestPathObserver);
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _observers.Add(observer);

            try
            {
                IEnumerable<PipeOutputPackage> otherPacakgesToInstall =
                    _historicalSemanticRegistry.NotifyObserverOfHistoricalRegistrations(observer);
                DoPackageInstallations(otherPacakgesToInstall);
            }
            catch
            {
                _killSwitchObserver.KillSignal();
                throw;
            }
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            try
            {
                IEnumerable<PipeOutputPackage> packagesToInsert = new[] {package};
                DoPackageInstallations(packagesToInsert);
            }
            catch (Exception e)
            {
                _killSwitchObserver.KillSignal();
                throw;
            }
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

        private sealed class KillSwitchObserver : ISemanticRegistryObserver
        {
            private bool _killed;

            public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
            {
                if (_killed)
                {
                    throw new InvalidProgramException();
                }

                return null;
            }

            public void KillSignal()
            {
                _killed = true;
            }
        }

        public ISolver CreateSolver()
        {
            return _shortestPathObserver;
        }
    }
}