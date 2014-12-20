using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly Dictionary<InputOutputPair, PipeExtension> _installedPipes =
            new Dictionary<InputOutputPair, PipeExtension>();

        private readonly RegistryMediator _registryMediator =
            new RegistryMediator(SemanticRegistryObserverFactory.CreateInternalObservers());

        private IEnumerable<PipeExtension> IterateCurrentPipeExtensions()
        {
            return _installedPipes.Select(installedPipe => installedPipe.Value);
        }

        public ISemanticBroker CreateBroker()
        {
            var solver = new Solver();
            IEnumerable<PipeExtension> currentPipeExtensions = IterateCurrentPipeExtensions();

            foreach (PipeExtension currentPipeExtension in currentPipeExtensions)
            {
                solver.Install(currentPipeExtension);
            }

            return new Broker(solver);
        }

        public SemanticBuilder InstallPipe<TSource, TDestination>(Func<TSource, TDestination> processCallback)
        {
            if (processCallback == null)
            {
                throw new ArgumentNullException("processCallback");
            }

            var inputOutputPair = new InputOutputPair(typeof (TSource), typeof (TDestination));

            PipeExtension extension = CreatePipeExtension(processCallback);


            _installedPipes.Add(inputOutputPair, extension);


            PipeOutputPackage package = CreatePipeOutputPackage(processCallback);
            _registryMediator.AppendPackage(package);

            return this;
        }

        private static PipeExtension CreatePipeExtension<TSource, TDestination>(
            Func<TSource, TDestination> processCallback)
        {
            Func<object, object> wrappedProcessCallback = rawInput =>
            {
                var castedInput = (TSource) rawInput;
                return processCallback(castedInput);
            };
            var extension = new PipeExtension(typeof (TSource), typeof (TDestination), wrappedProcessCallback);
            return extension;
        }

        private static PipeOutputPackage CreatePipeOutputPackage<TSource, TDestination>(
            Func<TSource, TDestination> processCallback)
        {
            Func<object, object> wrappedProcessCallback = rawInput =>
            {
                var castedInput = (TSource) rawInput;
                return processCallback(castedInput);
            };

            var package = new PipeOutputPackage(typeof (TSource), typeof (TDestination), wrappedProcessCallback);
            return package;
        }

        public void RegisterObserver(ISemanticRegistryObserver observer)
        {
            _registryMediator.AppendObserver(observer);
        }

        private sealed class InputOutputPair : Tuple<Type, Type>
        {
            public InputOutputPair(Type item1, Type item2) : base(item1, item2)
            {
            }
        }
    }
}