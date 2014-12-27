using System;
using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal class AdvertiveContravarianceChainObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.InputType.IsEnumerable())
                yield return ProcesType(package.InputType);

            if (package.OutputType.IsEnumerable())
                yield return ProcesType(package.OutputType);
        }

        private PipeOutputPackage ProcesType(Type type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }
    }
}