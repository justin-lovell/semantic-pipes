using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ArrayEnumberableConversionStrategy : IEnumberableConversionStrategy
    {
        private static readonly MethodInfo GenericToArrayMethodInfo = typeof (Enumerable).GetMethod("ToArray");

        public bool ShouldExpandUponIncomingPackage(PipeOutputPackage package)
        {
            return !package.OutputType.IsArray;
        }

        public Type CreateTargetOutputType(Type elementType)
        {
            return elementType.MakeArrayType();
        }

        public object DoConversion(Type elementType, IEnumerable input)
        {
            var closedMethodInfo = GenericToArrayMethodInfo.MakeGenericMethod(elementType);
            return closedMethodInfo.Invoke(input, new object[] {input});
        }
    }
}