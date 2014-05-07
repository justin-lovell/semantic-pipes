using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    public interface IPipeExtension
    {
        IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType);
    }
}