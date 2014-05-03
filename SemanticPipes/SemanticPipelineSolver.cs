using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class SemanticPipelineSolver
    {
        private readonly List<ISemanticRegistry> _registries = new List<ISemanticRegistry>();

        public void AppendRegistry(ISemanticRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException("registry");

            _registries.Add(registry);
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");
            if (outputType == null) throw new ArgumentNullException("outputType");

            GuardAgainstOperationsWithNoRegistriesAppended();

            var solvedPackage =
                (from registry in _registries
                    let packages = registry.PipeFrom(inputType)
                    where packages != null
                    from package in packages
                    where DoesPackageMatchInputAndOutput(package, inputType, outputType)
                    select package).FirstOrDefault();

            GuardAgainstUnsolveableInputOutputResolution(inputType, outputType, solvedPackage);

            return solvedPackage;
        }

        private static bool DoesPackageMatchInputAndOutput(PipeOutputPackage package, Type inputType, Type outputType)
        {
            return package.InputType == inputType && package.OutputType == outputType;
        }

        private static void GuardAgainstUnsolveableInputOutputResolution(
            Type inputType, Type outputType, PipeOutputPackage solvedPackage)
        {
            if (solvedPackage != null) return;

            string message = string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                inputType, outputType);
            throw new NotImplementedException(message);
        }

        private void GuardAgainstOperationsWithNoRegistriesAppended()
        {
            if (_registries.Count != 0) return;
            throw new NotSupportedException("No registries have been appended");
        }
    }
}