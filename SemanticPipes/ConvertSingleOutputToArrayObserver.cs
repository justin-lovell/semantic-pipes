using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal class ConvertSingleOutputToArrayObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.OutputType.IsArray)
            {
                yield break;
            }

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
            if (!inputType.IsClass)
            {
                return null;
            }

            Type outputType = inputType.MakeArrayType();
            Func<object, object> processCallbackFunc = value =>
            {
                Array instance = Array.CreateInstance(inputType, 1);
                instance.SetValue(value, 0);
                return instance;
            };

            return PipeOutputPackage.Infer(basedOffPackage, inputType, outputType, processCallbackFunc);
        }
    }
}