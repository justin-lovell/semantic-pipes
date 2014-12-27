using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes
{
    internal static class BrokerTransformerFactory
    {
        private static readonly IBrokerTransformer[] Transformers =
        {
            new EnumerableIntoList(),
            new EnumerableIntoArray(), 
        };

        public static Func<object, object> ConvertFor(Type actualType, Type requestedType)
        {
            if (actualType == requestedType)
            {
                return o => o;
            }

            var selectedTransforms =
                from tranformer in Transformers
                where tranformer.CanTransform(actualType, requestedType)
                select tranformer.CreateTransformingFunc(actualType, requestedType);

            return selectedTransforms.FirstOrDefault()
                   ?? (o => o);
        }
    }

    internal class EnumerableIntoArray : IBrokerTransformer
    {
        public bool CanTransform(Type actualType, Type requestedType)
        {
            return requestedType.IsArray;
        }

        public Func<object, object> CreateTransformingFunc(Type actualType, Type requestedType)
        {
            Type elementType = requestedType.ExtractEnumerableElementType();

            MethodInfo genericMethodInfo = typeof(Enumerable).GetMethod("ToArray");
            MethodInfo closedMethodInfo = genericMethodInfo.MakeGenericMethod(elementType);

            return o => closedMethodInfo.Invoke(o, new[] { o });
        }
    }

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

    internal interface IBrokerTransformer
    {
        bool CanTransform(Type actualType, Type requestedType);

        Func<object, object> CreateTransformingFunc(Type actualType, Type requestedType);
    }
}