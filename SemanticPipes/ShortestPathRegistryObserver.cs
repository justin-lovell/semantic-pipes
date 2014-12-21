using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal class ShortestPathRegistryObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            // todo: register into curry POP (key: output, second key: input)
            // todo: note: check the weights
            // todo: possibly recursively update all the curried POP's

            // todo: curry into new POP's (where existing outputs == package.InputType)
            // todo: take curried POP and then check if i/o is shorter path to currently known


            throw new NotImplementedException();
        }
    }
}