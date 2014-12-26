using System;
using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal sealed class AdvertiseOutputInheritanceChainObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            Type outputType = package.OutputType;

            if (outputType.IsEnumerable())
            {
                yield break;
            }

            if (outputType.BaseType != null && outputType.BaseType != typeof (object))
            {
                yield return CreateReturnPackage(package, outputType.BaseType);
            }

            foreach (Type interfaceImplementation in outputType.GetInterfaces())
            {
                yield return CreateReturnPackage(package, interfaceImplementation);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private static PipeOutputPackage CreateReturnPackage(PipeOutputPackage package, Type outputType)
        {
            return PipeOutputPackage.Infer(package, package.InputType, outputType, package.ProcessInput);
        }
    }
}