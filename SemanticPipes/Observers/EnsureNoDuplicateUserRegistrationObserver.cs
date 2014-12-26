using System;
using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal class EnsureNoDuplicateUserRegistrationObserver : ISemanticRegistryObserver
    {
        private readonly Dictionary<Tuple<Type, Type>, PipeOutputPackage> _registeredPackages =
            new Dictionary<Tuple<Type, Type>, PipeOutputPackage>();

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (!package.IsFromUserRegistration())
            {
                yield break;
            }

            var keyToSearchFor = new Tuple<Type, Type>(package.InputType, package.OutputType);

            if (_registeredPackages.ContainsKey(keyToSearchFor))
            {
                RaiseException(keyToSearchFor);
            }

            _registeredPackages.Add(keyToSearchFor, package);
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private void RaiseException(Tuple<Type, Type> tuple)
        {
            string message = string.Format("The pipe input of '{0}' to ouput of '{1}' has already been installed.",
                tuple.Item1, tuple.Item2);
            throw new InvalidRegistryConfigurationException(message);
        }
    }
}