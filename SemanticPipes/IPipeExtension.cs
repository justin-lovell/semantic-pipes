using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SemanticPipes
{
    public interface IPipeExtension
    {
        IEnumerable<PipeOutputPackage> PipeFrom(Type sourceType);
    }
}
