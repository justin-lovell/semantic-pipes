using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SemanticPipes.Observers
{
    internal sealed class ConvertEnumerableToEnumerableObserver : ISemanticRegistryObserver
    {
        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.InputType.IsEnumerable() || package.OutputType.IsEnumerable())
            {
                yield break;
            }

            MethodInfo genericCastingMethodInfo = typeof (Enumerable).GetMethod("Cast", new[] {typeof (IEnumerable)});
            MethodInfo castingMethodInfo = genericCastingMethodInfo.MakeGenericMethod(package.OutputType);

            Type inputType = typeof (IEnumerable<>).MakeGenericType(package.InputType);
            Type outputType = typeof (IEnumerable<>).MakeGenericType(package.OutputType);
            Func<object, object> processCallbackFunc = rawInputStream =>
            {
                var inputEnumerable = (IEnumerable) rawInputStream;
                IEnumerable<object> pipe =
                    from input in inputEnumerable.Cast<object>()
                    select package.ProcessInput(input);

                return castingMethodInfo.Invoke(pipe, new object[] {pipe});
            };
            yield return PipeOutputPackage.Infer(package, inputType, outputType, processCallbackFunc);
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }
    }
}