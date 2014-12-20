using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly Dictionary<InputOutputPair, PipeExtension> _installedPipes =
            new Dictionary<InputOutputPair, PipeExtension>();

        private readonly List<EventHandler<SemanticPipeInstalledEventArgs>> _pipeInstalledHandlers =
            new List<EventHandler<SemanticPipeInstalledEventArgs>>();

        public SemanticBuilder()
        {
            PipeInstalled += BuilderPipeAddOnForCollections.InstallHandler;
        }

        public event EventHandler<SemanticPipeInstalledEventArgs> PipeInstalled
        {
            add
            {
                if (value == null)
                {
                    return;
                }

                IEnumerable<PipeExtension> currentPipeExtensions = IterateCurrentPipeExtensions();
                foreach (PipeExtension currentPipeExtension in currentPipeExtensions)
                {
                    SemanticPipeInstalledEventArgs eventArgs =
                        CreateInstalledEventArgsForPipeExtension(currentPipeExtension);
                    value(this, eventArgs);
                }

                _pipeInstalledHandlers.Add(value);
            }
            remove { _pipeInstalledHandlers.Remove(value); }
        }

        private SemanticPipeInstalledEventArgs CreateInstalledEventArgsForPipeExtension(PipeExtension pipeExtension)
        {
            return new SemanticPipeInstalledEventArgs(pipeExtension.AppendPackage, pipeExtension);
        }

        private void OnPipeInstalled(PipeExtension pipeExtension)
        {
            SemanticPipeInstalledEventArgs e = CreateInstalledEventArgsForPipeExtension(pipeExtension);

            foreach (var handler in _pipeInstalledHandlers)
            {
                handler(this, e);
            }
        }

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

            GuardDuplicateInputOutputPairRegistration<TSource, TDestination>(inputOutputPair);

            PipeExtension extension = CreatePipeExtension(processCallback);

            OnPipeInstalled(extension);
            _installedPipes.Add(inputOutputPair, extension);

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

        private void GuardDuplicateInputOutputPairRegistration<TSource, TDestination>(InputOutputPair inputOutputPair)
        {
            if (!_installedPipes.ContainsKey(inputOutputPair))
            {
                return;
            }

            string message = string.Format("The pipe input of '{0}' to ouput of '{1}' has already been installed.",
                typeof (TSource), typeof (TDestination));
            throw new InvalidRegistryConfigurationException(message);
        }

        private sealed class InputOutputPair : Tuple<Type, Type>
        {
            public InputOutputPair(Type item1, Type item2) : base(item1, item2)
            {
            }
        }
    }
}