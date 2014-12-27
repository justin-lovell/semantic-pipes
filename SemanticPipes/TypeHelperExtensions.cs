using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal static class TypeHelperExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type ExtractEnumerableElementType(this Type inputType)
        {
            var seenTypes = new Dictionary<Type, object>();
            var processingQueue = new Queue<Type>(new[] { inputType });

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
                    if (genericTypeDefinition == typeof(IEnumerable<>))
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