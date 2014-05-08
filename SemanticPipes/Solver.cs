using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class Solver
    {
        private readonly List<IPipeExtension> _pipeExtensions = new List<IPipeExtension>();

        public void Install(IPipeExtension pipeExtension)
        {
            if (pipeExtension == null) throw new ArgumentNullException("pipeExtension");
            _pipeExtensions.Add(pipeExtension);
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");
            if (outputType == null) throw new ArgumentNullException("outputType");

            GuardAgainstOperationsWithNoPipePackagesInstalled();


            PipeOutputPackage solvedPackage = null;

            var pipingTransversalMap = new Dictionary<Type, PipeOutputPackage[]>();
            var typesToVisitQueue = new Queue<Tuple<Type, PipeOutputPackage[]>>();

            typesToVisitQueue.Enqueue(new Tuple<Type, PipeOutputPackage[]>(inputType, null));

            while (solvedPackage == null && typesToVisitQueue.Count > 0)
            {
                Tuple<Type, PipeOutputPackage[]> currentTuple = typesToVisitQueue.Dequeue();

                if (pipingTransversalMap.ContainsKey(currentTuple.Item1))
                    continue;

                pipingTransversalMap.Add(currentTuple.Item1, currentTuple.Item2);

                IEnumerable<PipeOutputPackage> candidatePipesForOutput =
                    DiscoverPipePackagesAcceptingInputType(currentTuple.Item1);
                PipeOutputPackage[] currentWalkedPath = currentTuple.Item2 ?? new PipeOutputPackage[0];

                foreach (PipeOutputPackage pipeOutputPackage in candidatePipesForOutput)
                {
                    if (pipeOutputPackage.OutputType == outputType)
                    {
                        solvedPackage = CreateAggregatedPipeOutputPackage(currentTuple.Item2, pipeOutputPackage);
                        break;
                    }

                    if (pipingTransversalMap.ContainsKey(pipeOutputPackage.OutputType))
                    {
                        continue;
                    }

                    PipeOutputPackage[] newPathToDiscover = currentWalkedPath.Union(new[] {pipeOutputPackage}).ToArray();
                    var tupleToVisit = new Tuple<Type, PipeOutputPackage[]>(pipeOutputPackage.OutputType,
                        newPathToDiscover);
                    typesToVisitQueue.Enqueue(tupleToVisit);
                }
            }

            GuardAgainstUnsolveableInputOutputResolution(inputType, outputType, solvedPackage);

            return solvedPackage;
        }

        private PipeOutputPackage CreateAggregatedPipeOutputPackage(
            PipeOutputPackage[] currentWalkedPath,
            PipeOutputPackage lastOutputPackage)
        {
            if (currentWalkedPath == null)
                return lastOutputPackage;

            Type combinedInputType = currentWalkedPath[0].InputType;
            Type combinedOutputType = lastOutputPackage.OutputType;

            Func<object, object> processCombinationCallback = o =>
            {
                o = currentWalkedPath.Aggregate(o,
                    (current, pipeOutputPackage) => pipeOutputPackage.ProcessInput(current));
                return lastOutputPackage.ProcessInput(o);
            };

            return new PipeOutputPackage(combinedInputType, combinedOutputType, processCombinationCallback);
        }

        private IEnumerable<PipeOutputPackage> DiscoverPipePackagesAcceptingInputType(Type inputType)
        {
            return
                from pipeExtension in _pipeExtensions
                let pipeOutputPackages = pipeExtension.PipeFrom(inputType)
                where pipeOutputPackages != null
                from package in pipeOutputPackages
                where package != null
                      && package.InputType == inputType
                select package;
        }

        private static void GuardAgainstUnsolveableInputOutputResolution(
            Type inputType, Type outputType, PipeOutputPackage solvedPackage)
        {
            if (solvedPackage != null && solvedPackage.OutputType == outputType) return;

            string message = string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                inputType, outputType);
            throw new CannotResolveSemanticException(message);
        }

        private void GuardAgainstOperationsWithNoPipePackagesInstalled()
        {
            if (_pipeExtensions.Count != 0) return;
            throw new InvalidRegistryConfigurationException("No IPipePackage have been installed");
        }
    }
}