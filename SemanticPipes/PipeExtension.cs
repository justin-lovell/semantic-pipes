using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    public sealed class PipeExtension : IPipeExtension
    {
        private readonly List<PipeOutputPackage> _packages = new List<PipeOutputPackage>();

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

            foreach (PipeOutputPackage pipeOutputPackage in _packages)
            {
                yield return pipeOutputPackage;
            }
        }

        internal void AppendPackage(PipeOutputPackage package)
        {
            _packages.Add(package);
        }
    }
}