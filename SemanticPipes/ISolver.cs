using System;

namespace SemanticPipes
{
    internal interface ISolver
    {
        PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType);
    }
}