using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SemanticPipes.Observers
{
    internal sealed class ConvertEnumerableToEnumerableObserver : ISemanticRegistryObserver
    {
        private static readonly MethodInfo GenericCastingMethodInfo;

        static ConvertEnumerableToEnumerableObserver()
        {
            GenericCastingMethodInfo = typeof (Enumerable).GetMethod("Cast", new[] {typeof (IEnumerable)});
        }

        public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
        {
            if (package.InputType.IsEnumerable() || package.OutputType.IsEnumerable())
            {
                yield break;
            }

            MethodInfo castingMethodInfo = GenericCastingMethodInfo.MakeGenericMethod(package.OutputType);

            Type inputType = typeof (IEnumerable<>).MakeGenericType(package.InputType);
            Type outputType = typeof (IEnumerable<>).MakeGenericType(package.OutputType);
            PipeCallback processCallbackFunc = async (rawInputStream, broker) =>
            {
                var inputEnumerable = (IEnumerable) rawInputStream;
                var pipe =
                    from input in inputEnumerable.Cast<object>()
                    select package.ProcessInput(input, broker);

                var results = await Task.WhenAll(pipe);
                return castingMethodInfo.Invoke(results, new object[] {results});
            };
            yield return PipeOutputPackage.Infer(package, inputType, outputType, processCallbackFunc);
        }

        public IEnumerable<PipeOutputPackage> SiblingPackageLateBounded(ISemanticRegistryObserver siblingObserver)
        {
            return null;
        }
    }
}