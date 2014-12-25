using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal class ShortestPathRegistryObserver : ISemanticRegistryObserver, ISolver
    {
        private static readonly TypeComparer TypeComparerInstance = new TypeComparer();

        private readonly Dictionary<Type, List<Type>> _incomingEdges = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, List<Type>> _outgoingEdges = new Dictionary<Type, List<Type>>();

        private readonly Dictionary<Tuple<Type, Type>, PipeOutputPackage> _shortestTransistions =
            new Dictionary<Tuple<Type, Type>, PipeOutputPackage>();

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            UpdateShortestPath(package);
            return null;
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            var key = new Tuple<Type, Type>(inputType, outputType);

            PipeOutputPackage outputPackage;

            _shortestTransistions.TryGetValue(key, out outputPackage);
            GuardAgainstUnsolveableInputOutputResolution(inputType, outputType, outputPackage);
            return outputPackage;
        }

        private void UpdateShortestPath(PipeOutputPackage package)
        {
            bool isShorterPath = RegisterShortestTransistion(package);

            if (!isShorterPath)
            {
                return;
            }

            RegisterEdge(_outgoingEdges, package.InputType, package.OutputType);
            RegisterEdge(_incomingEdges, package.OutputType, package.InputType);

            RecursivelySplayOutgoingTransistions(package);
            RecursivelyBackFillIncomingTransitions(package);
        }

        private void RecursivelyBackFillIncomingTransitions(PipeOutputPackage package)
        {
            List<Type> nextIncomingNodes;

            if (!_incomingEdges.TryGetValue(package.InputType, out nextIncomingNodes))
            {
                return;
            }

            IEnumerable<PipeOutputPackage> bridgingPackages =
                from ancestorIncomingType in nextIncomingNodes
                where ancestorIncomingType != package.InputType
                let transitionKey = new Tuple<Type, Type>(ancestorIncomingType, package.InputType)
                let incomingPackage = _shortestTransistions[transitionKey]
                select PipeOutputPackage.Bridge(incomingPackage, package);

            RecursivelyUpdateShortestPath(bridgingPackages);
        }

        private void RecursivelySplayOutgoingTransistions(PipeOutputPackage package)
        {
            List<Type> nextOutgoingNodes;

            if (!_outgoingEdges.TryGetValue(package.OutputType, out nextOutgoingNodes))
            {
                return;
            }

            IEnumerable<PipeOutputPackage> bridgingPackages =
                from descendantOutgoingNode in nextOutgoingNodes
                where descendantOutgoingNode != package.OutputType
                let transitionKey = new Tuple<Type, Type>(package.OutputType, descendantOutgoingNode)
                let outgoingPackage = _shortestTransistions[transitionKey]
                select PipeOutputPackage.Bridge(package, outgoingPackage);

            RecursivelyUpdateShortestPath(bridgingPackages);
        }

        private void RecursivelyUpdateShortestPath(IEnumerable<PipeOutputPackage> bridgingPackages)
        {
            foreach (PipeOutputPackage bridgingPackage in bridgingPackages)
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

            int binarySearchIndex = listOfDestinationTypes.BinarySearch(destination, TypeComparerInstance);

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

        private static void GuardAgainstUnsolveableInputOutputResolution(
            Type inputType, Type outputType, PipeOutputPackage solvedPackage)
        {
            if (solvedPackage != null && solvedPackage.OutputType == outputType) return;

            string message = string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                inputType, outputType);
            throw new CannotResolveSemanticException(message);
        }

        private class TypeComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        }
    }
}