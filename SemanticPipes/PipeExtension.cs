using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal sealed class PipeExtension : IPipeExtension
    {
        public PipeExtension(Func<object, object> processCallback, Type sourceType, Type destinationType)
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