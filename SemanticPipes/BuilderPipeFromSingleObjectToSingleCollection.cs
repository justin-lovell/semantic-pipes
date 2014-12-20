using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class BuilderPipeFromSingleObjectToSingleItemList : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (typeof(IEnumerable).IsAssignableFrom(package.InputType))
            {
                yield break;
            }

            yield return ConvertToDataType(package.InputType);
            yield return ConvertToDataType(package.OutputType);
        }

        private PipeOutputPackage ConvertToDataType(Type outputType)
        {
            Type listDestinationType = typeof(List<>).MakeGenericType(outputType);
            Func<object, object> processCallbackFunc = o =>
            {
                var list = (IList) Activator.CreateInstance(listDestinationType);
                list.Add(o);
                return list;
            };
            return new PipeOutputPackage(outputType, listDestinationType, processCallbackFunc);
        }
    }
}