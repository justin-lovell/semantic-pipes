using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes
{
    internal class EnumerableIntoList : IBrokerTransformer
    {
        public bool CanTransform(Type actualType, Type requestedType)
        {
            return requestedType.IsGenericType
                   && requestedType.GetGenericTypeDefinition() == typeof (List<>);
        }

        public Func<object, object> CreateTransformingFunc(Type actualType, Type requestedType)
        {
            Type elementType = requestedType.ExtractEnumerableElementType();

            MethodInfo genericMethodInfo = typeof (Enumerable).GetMethod("ToList");
            MethodInfo closedMethodInfo = genericMethodInfo.MakeGenericMethod(elementType);

            return o => closedMethodInfo.Invoke(o, new[] {o});
        }
    }
}