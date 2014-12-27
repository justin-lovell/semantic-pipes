using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal class SimplifyOutputResolving : ISolver
    {
        private readonly ISolver _solver;

        public SimplifyOutputResolving(ISolver solver)
        {
            _solver = solver;
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            if (outputType.IsEnumerable())
            {
                Type elementType = outputType.ExtractEnumerableElementType();

                if (elementType != null)
                {
                    outputType = typeof (IEnumerable<>).MakeGenericType(elementType);
                }
            }

            return _solver.SolveAsPipePackage(inputType, outputType);
        }
    }
}