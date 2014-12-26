using System;
using System.Collections;

namespace SemanticPipes
{
    internal static class TypeHelperExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}