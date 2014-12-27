using System;
using System.Collections.Generic;

namespace SemanticPipes.Solvers
{
    internal sealed class GraphEdgedSolver : ISolver
    {
        private readonly Dictionary<Tuple<Type, Type>, PipeOutputPackage> _lookup =
            new Dictionary<Tuple<Type, Type>, PipeOutputPackage>();
 
        public GraphEdgedSolver(IEnumerable<KeyValuePair<Tuple<Type, Type>, PipeOutputPackage>> graphEdges)
        {
            foreach (var keyValuePair in graphEdges)
            {
                _lookup.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {

            var key = new Tuple<Type, Type>(inputType, outputType);
            
            PipeOutputPackage outputPackage;

            _lookup.TryGetValue(key, out outputPackage);
            GuardAgainstUnsolveableInputOutputResolution(inputType, outputType, outputPackage);
            return outputPackage;
        }
        
        private static void GuardAgainstUnsolveableInputOutputResolution(
            Type inputType, Type outputType, PipeOutputPackage solvedPackage)
        {
            if (solvedPackage != null && solvedPackage.OutputType == outputType) return;
        
            string message = string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                inputType, outputType);
            throw new CannotResolveSemanticException(message);
        }
    }
}