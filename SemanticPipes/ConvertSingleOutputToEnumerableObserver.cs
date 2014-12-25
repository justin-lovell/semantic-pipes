using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class ConvertSingleOutputToEnumerableObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (!IsEnumerableType(package.InputType))
            {
                yield return ConvertToDataType(package.InputType, package);
            }

            if (!IsEnumerableType(package.OutputType))
            {
                yield return ConvertToDataType(package.OutputType, package);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private PipeOutputPackage ConvertToDataType(Type inputType, PipeOutputPackage basedOffPackage)
        {
            Type outputType = typeof(List<>).MakeGenericType(inputType);
            Func<object, object> processCallbackFunc = o =>
            {
                var list = (IList) Activator.CreateInstance(outputType);
                list.Add(o);
                return list;
            };

            return PipeOutputPackage.Infer(basedOffPackage, inputType, outputType, processCallbackFunc);
        }
    }
}