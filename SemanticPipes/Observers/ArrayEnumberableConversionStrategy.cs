using System;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ArrayEnumberableConversionStrategy : IEnumberableConversionStrategy
    {
        public bool ShouldExpandUponIncomingPackage(PipeOutputPackage package)
        {
            return !package.OutputType.IsArray;
        }

        public Type CreateTargetOutputType(Type elementType)
        {
            return elementType.MakeArrayType();
        }

        public MethodInfo ClosedGenericMethodInfo(Type elementType)
        {
            MethodInfo genericToArrayMethodInfo = typeof (Enumerable).GetMethod("ToArray");
            MethodInfo closedToArrayMethodInfo = genericToArrayMethodInfo.MakeGenericMethod(elementType);
            return closedToArrayMethodInfo;
        }
    }
}