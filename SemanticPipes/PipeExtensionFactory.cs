using System;

namespace SemanticPipes
{
    public static class PipeExtensionFactory
    {
        public static IPipeExtension Process<TSource, TDestination>(Func<TSource, TDestination> processCallback)
        {
            return new GenericPipeExtension<TSource, TDestination>(processCallback);
        }
    }
}