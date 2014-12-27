using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ListEnumberableConversionStrategy : IEnumberableConversionStrategy
    {
        private static readonly MethodInfo GenericToListMethodInfo = typeof(Enumerable).GetMethod("ToList");

        public bool ShouldExpandUponIncomingPackage(PipeOutputPackage package)
        {
            if (package.InputType.IsEnumerable())
                return true;

            if (!package.OutputType.IsGenericType)
            {
                return true;
            }

            if (package.OutputType.GetGenericTypeDefinition() == typeof (List<>))
            {
                return package.OutputType.GetGenericArguments().First() != package.InputType;
            }

            return true;
        }

        public Type CreateTargetOutputType(Type elementType)
        {
            return typeof (List<>).MakeGenericType(elementType);
        }

        public object DoConversion(Type elementType, IEnumerable input)
        {
            var closedMethodInfo = GenericToListMethodInfo.MakeGenericMethod(elementType);
            return closedMethodInfo.Invoke(input, new object[] {input});
        }
    }
}