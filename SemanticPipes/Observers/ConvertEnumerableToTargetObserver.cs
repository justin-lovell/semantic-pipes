using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ConvertEnumerableToTargetObserver : ISemanticRegistryObserver
    {
        private readonly IEnumberableConversionStrategy _strategy;

        public ConvertEnumerableToTargetObserver(IEnumberableConversionStrategy strategy)
        {
            _strategy = strategy;
        }

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (!_strategy.ShouldExpandUponIncomingPackage(package))
            {
                yield break;
            }

            if (package.InputType.IsEnumerable())
            {
                yield return ProcessType(package.InputType, package);
            }

            if (package.OutputType.IsEnumerable())
            {
                yield return ProcessType(package.OutputType, package);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private PipeOutputPackage ProcessType(Type inputType, PipeOutputPackage package)
        {
            Type elementType = ExtractElementType(inputType);

            if (elementType == null)
            {
                return null;
            }

            Type outputType = _strategy.CreateTargetOutputType(elementType);
            MethodInfo closedToArrayMethodInfo = _strategy.ClosedGenericMethodInfo(elementType);
            Func<object, ISemanticBroker, object> processCallback =
                (input, broker) => closedToArrayMethodInfo.Invoke(input, new[] {input});

            return PipeOutputPackage.Infer(package, inputType, outputType, processCallback);
        }

        private Type ExtractElementType(Type inputType)
        {
            var seenTypes = new Dictionary<Type, object>();
            var processingQueue = new Queue<Type>(new[] {inputType});

            while (processingQueue.Count > 0)
            {
                Type currentType = processingQueue.Dequeue();
                if (seenTypes.ContainsKey(currentType))
                {
                    continue;
                }

                if (currentType.IsGenericType)
                {
                    Type genericTypeDefinition = currentType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof (IEnumerable<>))
                    {
                        Type argumentType = currentType.GetGenericArguments().First();
                        return argumentType;
                    }
                }

                seenTypes.Add(currentType, null);
                foreach (Type nextType in inputType.GetInterfaces())
                {
                    processingQueue.Enqueue(nextType);
                }
            }

            return null;
        }
    }
}