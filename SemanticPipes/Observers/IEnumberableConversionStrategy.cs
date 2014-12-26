using System;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal interface IEnumberableConversionStrategy
    {
        bool ShouldExpandUponIncomingPackage(PipeOutputPackage package);
        Type CreateTargetOutputType(Type elementType);
        MethodInfo ClosedGenericMethodInfo(Type elementType);
    }
}