using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class BuilderPipeFromNonEnumerableToSingleItemList : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (IsEnumerableType(package.InputType) || IsEnumerableType(package.OutputType))
            {
                yield break;
            }

            yield return ConvertToDataType(package.InputType, package.ChainingWeight());
            yield return ConvertToDataType(package.OutputType, package.ChainingWeight());
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private PipeOutputPackage ConvertToDataType(Type outputType, int weight)
        {
            Type listDestinationType = typeof(List<>).MakeGenericType(outputType);
            Func<object, object> processCallbackFunc = o =>
            {
                var list = (IList) Activator.CreateInstance(listDestinationType);
                list.Add(o);
                return list;
            };
            return new PipeOutputPackage(weight, outputType, listDestinationType, processCallbackFunc);
        }
    }
}