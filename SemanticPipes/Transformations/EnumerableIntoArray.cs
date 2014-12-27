using System;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Transformations
{
    internal class EnumerableIntoArray : IBrokerTransformer
    {
        public bool CanTransform(Type actualType, Type requestedType)
        {
            return requestedType.IsArray
                   && requestedType.GetElementType() == actualType.ExtractEnumerableElementType();
        }

        public Func<object, object> CreateTransformingFunc(Type actualType, Type requestedType)
        {
            Type elementType = requestedType.ExtractEnumerableElementType();

            MethodInfo genericMethodInfo = typeof(Enumerable).GetMethod("ToArray");
            MethodInfo closedMethodInfo = genericMethodInfo.MakeGenericMethod(elementType);

            return o => closedMethodInfo.Invoke(o, new[] { o });
        }
    }
}