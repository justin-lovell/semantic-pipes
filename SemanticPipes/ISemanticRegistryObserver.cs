using System.Collections.Generic;

namespace SemanticPipes
{
    public interface ISemanticRegistryObserver
    {
        IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package);
    }
}