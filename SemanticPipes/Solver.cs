using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal interface ISolver
    {
        PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType);
    }

//    internal sealed class Solver : ISolver
//    {
//        private readonly List<IPipeExtension> _pipeExtensions = new List<IPipeExtension>();
//
//        public void Install(IPipeExtension pipeExtension)
//        {
//            if (pipeExtension == null) throw new ArgumentNullException("pipeExtension");
//            _pipeExtensions.Add(pipeExtension);
//        }
//
//        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
//        {
//            if (inputType == null) throw new ArgumentNullException("inputType");
//            if (outputType == null) throw new ArgumentNullException("outputType");
//
//            GuardAgainstOperationsWithNoPipePackagesInstalled();
//
//            var solvedPackage = TransverseGraphUntilSolved(inputType, outputType);
//
//            GuardAgainstUnsolveableInputOutputResolution(inputType, outputType, solvedPackage);
//
//            return solvedPackage;
//        }
//
//        private PipeOutputPackage TransverseGraphUntilSolved(Type inputType, Type outputType)
//        {
//            PipeOutputPackage solvedPackage = null;
//
//            var pipingTransversalMap = new Dictionary<Type, PipeOutputPackage[]>();
//            var typesToVisitQueue = new Queue<Tuple<Type, PipeOutputPackage[]>>();
//
//            typesToVisitQueue.Enqueue(new Tuple<Type, PipeOutputPackage[]>(inputType, null));
//
//            while (solvedPackage == null && typesToVisitQueue.Count > 0)
//            {
//                Tuple<Type, PipeOutputPackage[]> currentTuple = typesToVisitQueue.Dequeue();
//
//                if (pipingTransversalMap.ContainsKey(currentTuple.Item1))
//                    continue;
//
//                pipingTransversalMap.Add(currentTuple.Item1, currentTuple.Item2);
//
//                solvedPackage = TransverseCurrentTuplePipePackages(typesToVisitQueue,
//                    pipingTransversalMap, currentTuple, outputType);
//            }
//
//            return solvedPackage;
//        }
//
//        private PipeOutputPackage TransverseCurrentTuplePipePackages(
//            Queue<Tuple<Type, PipeOutputPackage[]>> typesToVisitQueue,
//            Dictionary<Type, PipeOutputPackage[]> pipingTransversalMap, Tuple<Type, PipeOutputPackage[]> currentTuple,
//            Type outputTypeToMatch)
//        {
//            IEnumerable<PipeOutputPackage> candidatePipesForOutput =
//                DiscoverPipePackagesAcceptingInputType(currentTuple.Item1);
//
//            foreach (PipeOutputPackage pipeOutputPackage in candidatePipesForOutput)
//            {
//                if (DoesPackageMatchOutputType(outputTypeToMatch, pipeOutputPackage))
//                {
//                    return CreateAggregatedPipeOutputPackage(currentTuple.Item2, pipeOutputPackage);
//                }
//
//                if (pipingTransversalMap.ContainsKey(pipeOutputPackage.OutputType))
//                {
//                    continue;
//                }
//
//                IdentifyNextTupleToVisit(currentTuple, pipeOutputPackage, typesToVisitQueue);
//            }
//
//            return null;
//        }
//
//        private static void IdentifyNextTupleToVisit(Tuple<Type, PipeOutputPackage[]> currentTuple,
//            PipeOutputPackage pipeOutputPackage,
//            Queue<Tuple<Type, PipeOutputPackage[]>> typesToVisitQueue)
//        {
//            PipeOutputPackage[] currentWalkedPath = currentTuple.Item2 ?? new PipeOutputPackage[0];
//            PipeOutputPackage[] newPathToDiscover = currentWalkedPath.Union(new[] {pipeOutputPackage}).ToArray();
//            var tupleToVisit = new Tuple<Type, PipeOutputPackage[]>(pipeOutputPackage.OutputType,
//                newPathToDiscover);
//
//            typesToVisitQueue.Enqueue(tupleToVisit);
//        }
//
//        private PipeOutputPackage CreateAggregatedPipeOutputPackage(
//            PipeOutputPackage[] currentWalkedPath,
//            PipeOutputPackage lastOutputPackage)
//        {
//            if (currentWalkedPath == null)
//                return lastOutputPackage;
//
//            Type combinedInputType = currentWalkedPath[0].InputType;
//            Type combinedOutputType = lastOutputPackage.OutputType;
//
//            Func<object, object> processCombinationCallback = o =>
//            {
//                o = currentWalkedPath.Aggregate(o,
//                    (current, pipeOutputPackage) => pipeOutputPackage.ProcessInput(current));
//                return lastOutputPackage.ProcessInput(o);
//            };
//
//            return new PipeOutputPackage(1, combinedInputType, combinedOutputType, processCombinationCallback);
//        }
//
//        private IEnumerable<PipeOutputPackage> DiscoverPipePackagesAcceptingInputType(Type inputType)
//        {
//            return
//                from pipeExtension in _pipeExtensions
//                let pipeOutputPackages = pipeExtension.PipeFrom(inputType)
//                where pipeOutputPackages != null
//                from package in pipeOutputPackages
//                where package != null
//                      && DoesPackageMatchInputType(inputType, package)
//                select package;
//        }
//
//        private static bool DoesPackageMatchInputType(Type inputType, PipeOutputPackage package)
//        {
//            return package.InputType == inputType;
//        }
//
//        private static bool DoesPackageMatchOutputType(Type outputType, PipeOutputPackage package)
//        {
//            return package.OutputType == outputType;
//        }
//
//        private void GuardAgainstOperationsWithNoPipePackagesInstalled()
//        {
//            if (_pipeExtensions.Count != 0) return;
//            throw new InvalidRegistryConfigurationException("No IPipePackage have been installed");
//        }
//    }
}