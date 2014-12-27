using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes.Transformations
{
    internal static class TransformerFactory
    {
        private static readonly IBrokerTransformer[] Transformers =
        {
            new EnumerableIntoList(),
            new EnumerableIntoArray()
        };

        private static readonly Dictionary<Tuple<Type, Type>, Func<object, object>> Cache =
            new Dictionary<Tuple<Type, Type>, Func<object, object>>();

        private static readonly object LockObject = new object();

        public static Func<object, object> ConvertFor(Type actualType, Type requestedType)
        {
            if (actualType == requestedType)
            {
                return o => o;
            }

            var key = new Tuple<Type, Type>(actualType, requestedType);
            Func<object, object> func;

            if (Cache.TryGetValue(key, out func))
            {
                return func;
            }

            func = CreateMappingFunc(actualType, requestedType);

            lock (LockObject)
            {
                if (!Cache.ContainsKey(key))
                {
                    Cache.Add(key, func);
                }
            }

            return func;
        }

        private static Func<object, object> CreateMappingFunc(Type actualType, Type requestedType)
        {
            IEnumerable<Func<object, object>> selectedTransforms =
                from tranformer in Transformers
                where tranformer.CanTransform(actualType, requestedType)
                select tranformer.CreateTransformingFunc(actualType, requestedType);

            return selectedTransforms.FirstOrDefault()
                   ?? (o => o);
        }
    }
}