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

            var isOutputEnumerable = IsEnumerableType(package.OutputType);

            if (isOutputEnumerable)
            {
                yield break;
            }

            yield return ConvertToDataType(package.OutputType, package);

            var isInputEnumerable = IsEnumerableType(package.InputType);
            if (!isInputEnumerable)
            {
                yield return ConvertToDataType(package.InputType, package);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof (IEnumerable).IsAssignableFrom(type);
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