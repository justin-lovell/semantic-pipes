using System;
using System.Collections;

namespace SemanticPipes.Observers
{
    internal interface IEnumberableConversionStrategy
    {
        bool ShouldExpandUponIncomingPackage(PipeOutputPackage package);
        Type CreateTargetOutputType(Type elementType);
        object DoConversion(Type elementType, IEnumerable input);
    }
}