using System;
using System.Collections.Generic;

namespace SemanticPipes
{
    internal class SimplifyEnumerableTypeResolving : ISolver
    {
        private readonly ISolver _solver;

        public SimplifyEnumerableTypeResolving(ISolver solver)
        {
            _solver = solver;
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            inputType = SimplifyType(inputType);
            outputType = SimplifyType(outputType);

            return _solver.SolveAsPipePackage(inputType, outputType);
        }

        private static Type SimplifyType(Type type)
        {
            if (type.IsEnumerable())
            {
                Type elementType = type.ExtractEnumerableElementType();

                if (elementType != null)
                {
                    return typeof (IEnumerable<>).MakeGenericType(elementType);
                }
            }

            return type;
        }
    }
}