using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    public sealed class SemanticBuilder
    {
        private readonly Dictionary<InputOutputPair, PipeExtension> _installedPipes =
            new Dictionary<InputOutputPair, PipeExtension>();

        public ISemanticBroker CreateBroker()
        {
            var solver = new Solver();

            foreach (var installedPipe in _installedPipes)
            {
                solver.Install(installedPipe.Value);
            }

            return new Broker(solver);
        }

        public SemanticBuilder InstallPipe<TSource, TDestination>(Func<TSource, TDestination> processCallback)
        {
            if (processCallback == null) throw new ArgumentNullException("processCallback");

            var inputOutputPair = new InputOutputPair(typeof (TSource), typeof (TDestination));

            GuardDuplicateInputOutputPairRegistration<TSource, TDestination>(inputOutputPair);

            var extension = CreatePipeExtension(processCallback);
            _installedPipes.Add(inputOutputPair, extension);

            return this;
        }

        private static PipeExtension CreatePipeExtension<TSource, TDestination>(Func<TSource, TDestination> processCallback)
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
            if (!_installedPipes.ContainsKey(inputOutputPair)) return;

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