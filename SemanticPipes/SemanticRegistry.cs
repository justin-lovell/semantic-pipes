using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    class SemanticRegistry : ISemanticRegistry
    {
        public void Install(IPipeExtension extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            throw new System.NotImplementedException();
        }

        public IEnumerable<PipePackageOption> PipeFrom(Type sourceType)
        {
            throw new NotImplementedException();
        }
    }
}