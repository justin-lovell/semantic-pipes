using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ConvertEnumerableToArrayObserver : ISemanticRegistryObserver
    {

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.OutputType.IsArray)
            {
                yield break;
            }

            if (package.InputType.IsEnumerable())
            {
                yield return ProcessTypeIntoArray(package.InputType, package);
            }

            if (package.OutputType.IsEnumerable())
            {
                yield return ProcessTypeIntoArray(package.OutputType, package);
            }
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        private PipeOutputPackage ProcessTypeIntoArray(Type inputType, PipeOutputPackage package)
        {
            Type elementType = ExtractElementType(inputType);

            if (elementType == null)
            {
                return null;
            }

            Type arrayType = elementType.MakeArrayType();
            MethodInfo genericToArrayMethodInfo = typeof(Enumerable).GetMethod("ToArray");
            MethodInfo closedToArrayMethodInfo = genericToArrayMethodInfo.MakeGenericMethod(elementType);
            Func<object, object> processCallback = o => closedToArrayMethodInfo.Invoke(o, new[] {o});

            return PipeOutputPackage.Infer(package, inputType, arrayType, processCallback);
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