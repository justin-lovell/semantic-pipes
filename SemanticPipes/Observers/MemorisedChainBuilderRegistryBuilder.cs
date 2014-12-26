using System;
using System.Collections.Generic;

namespace SemanticPipes.Observers
{
    public sealed class MemorisedChainBuilderRegistryBuilder : ISemanticRegistryObserver
    {
        private readonly Dictionary<Tuple<Type, Type>, PipeOutputPackage> _packages =
            new Dictionary<Tuple<Type, Type>, PipeOutputPackage>();

        private readonly ISemanticRegistryObserver _wrappedObserver;

        public MemorisedChainBuilderRegistryBuilder(ISemanticRegistryObserver wrappedObserver)
        {
            _wrappedObserver = wrappedObserver;
        }

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            var key = new Tuple<Type, Type>(package.InputType, package.OutputType);
            PipeOutputPackage rememberedPackage;
            bool hasEncounteredPackageBefore = _packages.TryGetValue(key, out rememberedPackage);

            if (!hasEncounteredPackageBefore)
            {
                _packages.Add(key, package);
            }

            if (!hasEncounteredPackageBefore || package.Weight < rememberedPackage.Weight)
            {
                return _wrappedObserver.PipePackageInstalled(package);
            }

            return null;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return _wrappedObserver.SiblingPackageLateBounded(siblingObserver);
        }
    }
}