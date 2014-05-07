using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class GenericPipeExtension<TSource, TDestination> : IPipeExtension
    {
        private readonly Func<TSource, TDestination> _processCallback;

        public GenericPipeExtension(Func<TSource,TDestination> processCallback)
        {
            if (processCallback == null) throw new ArgumentNullException("processCallback");
            _processCallback = processCallback;
        }

        public IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType)
        {
            if (sourceType == null)
                return null;
            if (sourceType != typeof (TSource))
                return null;

            Func<object, object> processCallbackFunc = rawInput =>
            {
                var castedInput = (TSource) rawInput;
                return _processCallback(castedInput);
            };
            var package = new PipeOutputPackage(typeof (TSource), typeof (TDestination), processCallbackFunc);

            return new[] {package};
        }
    }
}