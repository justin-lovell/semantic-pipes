using System;

namespace SemanticPipes.Transformations
{
    internal interface IBrokerTransformer
    {
        bool CanTransform(Type actualType, Type requestedType);

        Func<object, object> CreateTransformingFunc(Type actualType, Type requestedType);
    }
}