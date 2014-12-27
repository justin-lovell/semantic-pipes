using System;
using System.Collections.Generic;

namespace SemanticPipes.Registries
{
    internal sealed class SimplifyEnumerableOutputRegistryMediator : IRegistryMediator
    {
        private readonly IRegistryMediator _nextMediator;

        public SimplifyEnumerableOutputRegistryMediator(IRegistryMediator nextMediator)
        {
            _nextMediator = nextMediator;
        }

        public void AppendObserver(ISemanticRegistryObserver observer)
        {
            _nextMediator.AppendObserver(observer);
        }

        public void AppendPackage(PipeOutputPackage package)
        {
            package = SimplifyInput(package);
            package = SimplifyOutput(package);
            _nextMediator.AppendPackage(package);
        }

        private static PipeOutputPackage SimplifyOutput(PipeOutputPackage package)
        {
            Type newOutputType = DetermineNewInterestType(package.OutputType);

            return newOutputType == package.OutputType
                ? package
                : PipeOutputPackage.Direct(package.InputType, newOutputType, package.ProcessInput);
        }

        private static PipeOutputPackage SimplifyInput(PipeOutputPackage package)
        {
            Type newInputType = DetermineNewInterestType(package.InputType);

            if (newInputType == package.InputType)
            {
                return package;
            }

            PipeCallback processCallbackFunc = (input, broker) =>
            {
                Func<object, object> transformFunc =
                    BrokerTransformerFactory.ConvertFor(newInputType, package.InputType);
                object revisedInput = transformFunc(input);
                return package.ProcessInput(revisedInput, broker);
            };
            return PipeOutputPackage.Direct(newInputType, package.OutputType, processCallbackFunc);
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