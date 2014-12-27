using System;
using System.Collections.Generic;

namespace SemanticPipes.Registries
{
    internal sealed class SimplifyEnumerablePipePackageOutputRegistryMediator : IRegistryMediator
    {
        private readonly IRegistryMediator _nextMediator;

        public SimplifyEnumerablePipePackageOutputRegistryMediator(IRegistryMediator nextMediator)
        {
            _nextMediator = nextMediator;
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _nextMediator.AppendObserver(observer);
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            Type newInputType = DetermineNewInterestType(package.InputType);
            Type newOutputType = DetermineNewInterestType(package.OutputType);

            if (newInputType != package.InputType || newOutputType != package.OutputType)
            {
                package = PipeOutputPackage.Direct(newInputType, newOutputType, package.ProcessInput);
            }

            _nextMediator.AppendPackage(package);
        }

        private static Type DetermineNewInterestType(Type type)
        {
            Type newTypeOfInterest = type;

            if (IsTypeOfInterest(type))
            {
                Type elementType = type.ExtractEnumerableElementType();

                if (elementType != null)
                {
                    newTypeOfInterest = typeof (IEnumerable<>).MakeGenericType(elementType);
                }
            }

            return newTypeOfInterest;
        }

        private static bool IsTypeOfInterest(Type type)
        {
            return type.IsEnumerable() &&
                   (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof (IEnumerable<>));
        }
    }
}