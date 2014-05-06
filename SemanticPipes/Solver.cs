using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticPipes
{
    internal sealed class Solver
    {
        private readonly List<IPipeExtension> _pipeExtensions = new List<IPipeExtension>(); 

        public void Install(IPipeExtension pipeExtension)
        {
            if (pipeExtension == null) throw new ArgumentNullException("pipeExtension");
            _pipeExtensions.Add(pipeExtension);
        }

        public PipeOutputPackage SolveAsPipePackage(Type inputType, Type outputType)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");
            if (outputType == null) throw new ArgumentNullException("outputType");

            GuardAgainstOperationsWithNoPipePackagesInstalled();

            var solvedPackage =
                (from pipeExtension in _pipeExtensions
                    let pipeOutputPackages = pipeExtension.PipeFrom(inputType)
                    where pipeOutputPackages != null
                    from package in pipeOutputPackages
                    where package != null
                          && DoesPackageMatchInputAndOutput(package, inputType, outputType)
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

        private void GuardAgainstOperationsWithNoPipePackagesInstalled()
        {
            if (_pipeExtensions.Count != 0) return;
            throw new InvalidRegistryConfigurationException("No IPipePackage have been installed");
        }
    }
}