using System;
using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal sealed class ConvertSingleOutputToEnumerableObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (!package.InputType.IsEnumerable() && package.OutputType.IsEnumerable())
            {
                Type selfEnumerableType = typeof (IEnumerable<>).MakeGenericType(package.InputType);

                if (selfEnumerableType.IsAssignableFrom(package.OutputType))
                {
                    // this fingerprint shows that we are registering ourselves
                    yield break;
                }
            }

            if (!package.OutputType.IsEnumerable())
            {
                yield return ConvertToDataType(package.OutputType, package);
            }

            if (!package.InputType.IsEnumerable())
            {
                yield return ConvertToDataType(package.InputType, package);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private PipeOutputPackage ConvertToDataType(Type inputType, PipeOutputPackage basedOffPackage)
        {
            Func<object, ISemanticBroker, object> processCallbackFunc = (input, broker) =>
            {
                Array array = Array.CreateInstance(inputType, 1);
                array.SetValue(input, 0);
                return array;
            };

            Type outputType = typeof (IEnumerable<>).MakeGenericType(inputType);
            return PipeOutputPackage.Infer(basedOffPackage, inputType, outputType, processCallbackFunc);
        }
    }
}