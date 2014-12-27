using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes.Observers
{
    internal class IntoEnumberableConversionStrategy : IEnumberableConversionStrategy
    {
        public bool ShouldExpandUponIncomingPackage(PipeOutputPackage package)
        {
            bool isInputOrOutputList =
                IsTypeGenericMatch(package.InputType, typeof (List<>))
                || IsTypeGenericMatch(package.OutputType, typeof (List<>));

            if (!isInputOrOutputList)
            {
                return false;
            }

            if (!IsTypeGenericMatch(package.OutputType, typeof (IEnumerable<>)))
            {
                return true;
            }

            // ensure that we are not matching upon our own POP's
            Type inputElementType = ExtractElementType(package.InputType);
            Type outputElementType = ExtractElementType(package.OutputType);

            return inputElementType != outputElementType;
        }

        public Type CreateTargetOutputType(Type elementType)
        {
            return typeof (IEnumerable<>).MakeGenericType(elementType);
        }

        public object DoConversion(Type elementType, IEnumerable input)
        {
            return input;
        }

        private static Type ExtractElementType(Type typeInspected)
        {
            Type enumerableType = typeInspected.GetInterfaces()
                .FirstOrDefault(interfaceType => IsTypeGenericMatch(interfaceType, typeof (IEnumerable<>)));

            return enumerableType == null
                ? null
                : enumerableType.GetGenericArguments().First();
        }

        private static bool IsTypeGenericMatch(Type type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}