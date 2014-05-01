using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    class SemanticRegistry : ISemanticRegistry
    {
        private readonly List<IPipeExtension> _pipeExtensions = new List<IPipeExtension>(); 

        public void Install(IPipeExtension extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            _pipeExtensions.Add(extension);
        }

        public IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType)
        {
            if (sourceType == null) throw new ArgumentNullException("sourceType");

            var pipeExtensions = _pipeExtensions;
            return pipeExtensions.SelectMany(pipeExtension => ExtractValidOutputPackages(sourceType, pipeExtension));
        }

        private static IEnumerable<PipeOutputPackage> ExtractValidOutputPackages(Type sourceType, IPipeExtension pipeExtension)
        {
            return pipeExtension.PipeFrom(sourceType).Where(package => package.InputType == sourceType);
        }
    }
}