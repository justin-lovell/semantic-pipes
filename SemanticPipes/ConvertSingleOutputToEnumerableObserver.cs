using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class ConvertSingleOutputToEnumerableObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.OutputType.IsEnumerable())
            {
                yield break;
            }

            yield return ConvertToDataType(package.OutputType, package);

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