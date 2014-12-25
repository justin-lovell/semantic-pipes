using System.Collections.Generic;

namespace SemanticPipes
{
    public interface ISemanticRegistryObserver
    {
        IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package);
        IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver);
    }
}