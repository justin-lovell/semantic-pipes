using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    public sealed class PipeExtension : IPipeExtension
    {
        internal PipeExtension(Type sourceType, Type destinationType, Func<object, object> processCallback)
        {
            ProcessCallback = processCallback;
            SourceType = sourceType;
            DestinationType = destinationType;
        }

        public Func<object, object> ProcessCallback { get; private set; }

        public Type SourceType { get; private set; }

        public Type DestinationType { get; private set; }

        public IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType)
        {
            if (sourceType != SourceType) yield break;

            yield return new PipeOutputPackage(SourceType, DestinationType, ProcessCallback);
        }
    }
}