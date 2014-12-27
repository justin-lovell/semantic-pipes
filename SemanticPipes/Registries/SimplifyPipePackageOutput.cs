using System;
using System.Collections.Generic;

namespace SemanticPipes.Registries
{
    internal sealed class SimplifyPipePackageOutput : IRegistryMediator
    {
        private readonly IRegistryMediator _nextMediator;

        public SimplifyPipePackageOutput(IRegistryMediator nextMediator)
        {
            _nextMediator = nextMediator;
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _nextMediator.AppendObserver(observer);
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            if (IsPackageOfInterest(package.OutputType))
            {
                Type elementType = package.OutputType.ExtractEnumerableElementType();
                Type newOutputType = typeof (IEnumerable<>).MakeGenericType(elementType);

                package = PipeOutputPackage.Direct(package.InputType, newOutputType, package.ProcessInput);
            }

            _nextMediator.AppendPackage(package);
        }

        private static bool IsPackageOfInterest(Type outputType)
        {
            return outputType.IsEnumerable() &&
                   (!outputType.IsGenericType || outputType.GetGenericTypeDefinition() != typeof (IEnumerable<>));
        }
    }
}