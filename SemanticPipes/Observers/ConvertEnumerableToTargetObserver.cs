using System;
using System.Collections;
using System.Collections.Generic;

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
            Type elementType = inputType.ExtractEnumerableElementType();

            if (elementType == null)
            {
                return null;
            }

            Type outputType = _strategy.CreateTargetOutputType(elementType);
            PipeCallback processCallback =
                (input, broker) => _strategy.DoConversion(elementType, (IEnumerable) input).IntoTaskResult();

            return PipeOutputPackage.Infer(package, inputType, outputType, processCallback);
        }
    }
}