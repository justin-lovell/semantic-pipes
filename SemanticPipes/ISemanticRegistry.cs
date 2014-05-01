using System;
using System.Collections;
using System.Collections.Generic;

namespace SemanticPipes
{
    public interface ISemanticRegistry
    {
        void Install(IPipeExtension extension);
        IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType);
    }
}