﻿using System;
using System.Threading.Tasks;
using SemanticPipes.Transformations;

namespace SemanticPipes
{
    internal sealed class Broker : ISemanticBroker
    {
        private readonly ISolver _solver;

        internal Broker(ISolver solver)
        {
            _solver = solver;
        }

        public ISemanticOpenPipe On<TSource>(TSource source)
            where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");

            return new SolvingPipe<TSource>(this, _solver, source);
        }

        private class SolvingPipe<TSource> : ISemanticOpenPipe
        {
            private readonly ISolver _solver;
            private readonly object _source;
            private readonly Broker _broker;

            public SolvingPipe(Broker broker, ISolver solver, object source)
            {
                _solver = solver;
                _source = source;
                _broker = broker;
            }

            public Task<TDestination> Output<TDestination>()
            {
                PipeOutputPackage solvedPipePackage = _solver.SolveAsPipePackage(typeof(TSource), typeof(TDestination));
                Func<object, object> transformInput =
                    TransformerFactory.ConvertFor(typeof(TSource), solvedPipePackage.InputType);
                Func<object, object> transformOutput =
                    TransformerFactory.ConvertFor(solvedPipePackage.OutputType, typeof (TDestination));

                var input = transformInput(_source);

                return
                    solvedPipePackage.ProcessInput(input, _broker)
                        .ContinueWith(task => (TDestination)transformOutput(task.Result));
            }
        }
    }
}