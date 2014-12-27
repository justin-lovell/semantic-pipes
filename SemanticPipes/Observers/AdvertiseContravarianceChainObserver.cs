using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticPipes.Observers
{
    internal class AdvertiseContravarianceChainObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            IEnumerable<PipeOutputPackage> result = Enumerable.Empty<PipeOutputPackage>();

            if (package.InputType.IsEnumerable())
            {
                result = result.Concat(ProcessType(package.InputType, package));
            }

            if (package.OutputType.IsEnumerable())
            {
                result = result.Concat(ProcessType(package.OutputType, package));
            }

            return result;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private IEnumerable<PipeOutputPackage> ProcessType(Type enumerableType, PipeOutputPackage package)
        {
            Type elementType = enumerableType.ExtractEnumerableElementType();

            if (elementType == null)
            {
                yield break;
            }

            if (elementType.BaseType != null && elementType.BaseType != typeof (object))
            {
                yield return ExposeConversion(elementType, elementType.BaseType, package);
            }

            IEnumerable<PipeOutputPackage> interfaceInference =
                from interfaceType in elementType.GetInterfaces()
                select ExposeConversion(elementType, interfaceType, package);

            foreach (PipeOutputPackage inferencePackage in interfaceInference)
            {
                yield return inferencePackage;
            }
        }

        private PipeOutputPackage ExposeConversion(Type elementType, Type baseType, PipeOutputPackage package)
        {
            Type inputType = typeof (IEnumerable<>).MakeGenericType(elementType);
            Type outputType = typeof (IEnumerable<>).MakeGenericType(baseType);

            return PipeOutputPackage.Infer(package, inputType, outputType, (input, broker) => Task.FromResult(input));
        }
    }
}