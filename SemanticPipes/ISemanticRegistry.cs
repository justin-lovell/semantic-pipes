using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    public interface ISemanticRegistry
    {
        void Install(IPipeExtension extension);
        IEnumerable<PipePackageOption> PipeFrom(Type sourceType);
    }
}