using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal class ShortestPathRegistryObserver : ISemanticRegistryObserver
    {
//        private readonly Dictionary<Type, List<PipeOutputPackage>> _appendixOfPackagesByOutput =
//            new Dictionary<Type, List<PipeOutputPackage>>();
//
//        private readonly Dictionary<Type, List<PipeOutputPackage>> _appendixOfPackagesByInput =
//            new Dictionary<Type, List<PipeOutputPackage>>();


        private readonly Dictionary<Type, List<Type>> _outgoingEdges = new Dictionary<Type, List<Type>>();

        //private readonly Dictionary<Type, List<Type>> _incomingEdges = new Dictionary<Type, List<Type>>();

        private readonly Dictionary<Tuple<Type, Type>, PipeOutputPackage> _shortestTransistions =
            new Dictionary<Tuple<Type, Type>, PipeOutputPackage>();

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            UpdateShortestPath(package);
            return null;

            // todo: curry into new POP's (where existing outputs == package.InputType || inputs == package.Output)
            // todo: take curried POP and then check if i/o is shorter path to currently known


            // todo: register into curry POP (key: output, second key: input)
            // todo: note: check the weights
            // todo: possibly recursively update all the curried POP's
        }

        private void UpdateShortestPath(PipeOutputPackage package)
        {
            bool isShorterPath = RegisterShortestTransistion(package);

            if (!isShorterPath)
            {
                return;
            }

            RegisterEdge(_outgoingEdges, package.InputType, package.OutputType);
            RecursivelySplayOutgoingTransistions(package);

            // only go into it by "one"
//                EnrollIntoAppendix(_appendixOfPackagesByOutput, package.OutputType, package);
//
//                // only go back by "one"
//                EnrollIntoAppendix(_appendixOfPackagesByInput, package.InputType, package);

            // recursively Enroll package
        }

        private void RecursivelySplayOutgoingTransistions(PipeOutputPackage package)
        {
            List<Type> nextOutgoingNodes;

            if (!_outgoingEdges.TryGetValue(package.OutputType, out nextOutgoingNodes))
            {
                return;
            }

            var bridgingPackages =
                from nextOutgoingNode in nextOutgoingNodes
                let transitionKey = new Tuple<Type, Type>(package.OutputType, nextOutgoingNode)
                let outgoingPackage = _shortestTransistions[transitionKey]
                select PipeOutputPackage.Bridge(package, outgoingPackage);

            foreach (var bridgingPackage in bridgingPackages)
            {
                UpdateShortestPath(bridgingPackage);
            }
        }

        private void RegisterEdge(Dictionary<Type, List<Type>> edges, Type source, Type destination)
        {
            List<Type> listOfDestinationTypes;
            bool foundOtherEdges = edges.TryGetValue(source, out listOfDestinationTypes);

            if (!foundOtherEdges)
            {
                listOfDestinationTypes = new List<Type> {destination};
                edges.Add(source, listOfDestinationTypes);
                return;
            }

            int binarySearchIndex = listOfDestinationTypes.BinarySearch(destination);

            if (binarySearchIndex >= 0)
            {
                return;
            }

            listOfDestinationTypes.Insert(~binarySearchIndex, destination);
        }

        private bool RegisterShortestTransistion(PipeOutputPackage package)
        {
            var keyToLookup = new Tuple<Type, Type>(package.InputType, package.OutputType);
            PipeOutputPackage currentlyKnownPath;

            if (_shortestTransistions.TryGetValue(keyToLookup, out currentlyKnownPath))
            {
                if (package.Weight < currentlyKnownPath.Weight)
                {
                    _shortestTransistions[keyToLookup] = package;
                    return true;
                }
            }
            else
            {
                _shortestTransistions.Add(keyToLookup, package);
                return true;
            }

            return false;
        }

//        private static void EnrollIntoAppendix(Dictionary<Type, List<PipeOutputPackage>> appendixOfPackages,
//            Type lookupType, PipeOutputPackage package)
//        {
//            List<PipeOutputPackage> appendix;
//            if (!appendixOfPackages.TryGetValue(lookupType, out appendix))
//            {
//                appendix = new List<PipeOutputPackage>();
//                appendixOfPackages.Add(lookupType, appendix);
//            }
//
//            appendix.Add(package);
//        }
    }
}