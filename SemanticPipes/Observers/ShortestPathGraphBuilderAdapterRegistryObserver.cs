using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    internal class ShortestPathGraphBuilderAdapterRegistryObserver : ISemanticRegistryObserver
    {
        private readonly ShortestPathGraphBuilder _graphBuilder;

        public ShortestPathGraphBuilderAdapterRegistryObserver(ShortestPathGraphBuilder graphBuilder)
        {
            _graphBuilder = graphBuilder;
        }

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            _graphBuilder.UpdateShortestPath(package);
            return null;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }
    }
}