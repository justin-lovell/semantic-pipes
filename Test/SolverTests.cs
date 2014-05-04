using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SolverTests
    {
        [TestCase(typeof (string), typeof (int))]
        [TestCase(typeof (int), typeof (string))]
        public void SolveAsPipePackage_WhenTheInputAndOutputPairCannotBeResolved_ItShouldThrowNotImplementedExcpetion(
            Type inputType, Type outputType)
        {
            var pipeExtension = A.Fake<IPipeExtension>();
            A.CallTo(() => pipeExtension.PipeFrom(inputType)).Returns(null);

            var solver = new Solver();
            solver.Install(pipeExtension);

            var notImplementedException =
                Assert.Throws<NotImplementedException>(() => solver.SolveAsPipePackage(inputType, outputType));

            string expectedExceptionMessage =
                string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                    inputType, outputType);
            Assert.AreEqual(expectedExceptionMessage, notImplementedException.Message);
        }

        [TestCase(typeof (string), typeof (int))]
        [TestCase(typeof (int), typeof (string))]
        public void SolveAsPipePackage_WhenSearchingForAnInputOutputPair_ItShouldGuardAgainstInvalidPackageCombinations(
            Type solveToInputType, Type solveToOutputType)
        {
            Type inputType = typeof (string);
            Type outputType = typeof (string);

            if (inputType == solveToInputType && outputType == solveToOutputType)
            {
                Assert.Fail("The test is designed not to have a string/string combination");
            }

            var expectedPackage = new PipeOutputPackage(inputType, outputType, o => o.ToString());

            var pipeExtension = A.Fake<IPipeExtension>();
            A.CallTo(() => pipeExtension.PipeFrom(solveToInputType)).Returns(new[] {expectedPackage});

            var solver = new Solver();
            solver.Install(pipeExtension);

            var notImplementedException =
                Assert.Throws<NotImplementedException>(
                    () => solver.SolveAsPipePackage(solveToInputType, solveToOutputType));

            string expectedExceptionMessage =
                string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                    solveToInputType, solveToOutputType);
            Assert.AreEqual(expectedExceptionMessage, notImplementedException.Message);
        }

        [Test]
        public void AppendRegistry_WhenNullToRegistryParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var solver = new Solver();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => solver.AppendRegistry(null));

            Assert.AreEqual("registry", argumentNullException.ParamName);
        }

        [Test]
        public void AppendRegistry_WhenValidInstanceIsPassedIn_ItShouldAcceptIt()
        {
            var solver = new Solver();
            var registry = A.Fake<ISemanticRegistry>();

            A.CallTo(() => registry.PipeFrom(typeof (string))).Returns(null);

            solver.AppendRegistry(registry);
        }

        [Test]
        public void SolveAsPipePackage_WhenNoRegistryHasBeenAppended_ItShouldThrowNotSupportedExcpetion()
        {
            var solver = new Solver();

            Assert.Throws<NotSupportedException>(() => solver.SolveAsPipePackage(typeof (string), typeof (string)));
        }

        [Test]
        public void SolveAsPipePackage_WhenNullToInputTypeParameter_ItShouldThrowArgumentNullException()
        {
            var solver = new Solver();

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => solver.SolveAsPipePackage(null, typeof (string)));

            Assert.AreEqual("inputType", argumentNullException.ParamName);
        }

        [Test]
        public void SolveAsPipePackage_WhenNullToOutputTypeParameter_ItShouldThrowArgumentNullException()
        {
            var solver = new Solver();

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => solver.SolveAsPipePackage(typeof (string), null));

            Assert.AreEqual("outputType", argumentNullException.ParamName);
        }

        [Test]
        public void
            SolveAsPipePackage_WhenTheInputAndOutputPairCanBeResolved_ItShouldReturnCorrespondingPipeOutputPackage()
        {
            Type inputType = typeof (string);
            Type outputType = typeof (string);

            var expectedPackage = new PipeOutputPackage(inputType, outputType, o => null);

            var pipelineExtension = A.Fake<IPipeExtension>();
            A.CallTo(() => pipelineExtension.PipeFrom(inputType)).Returns(new[] {expectedPackage});

            var solver = new Solver();
            solver.Install(pipelineExtension);


            PipeOutputPackage solvedPackage = solver.SolveAsPipePackage(inputType, outputType);


            Assert.AreSame(expectedPackage, solvedPackage);
        }

        [Test]
        public void SolveAsPipePackage_WhenPipelinePackagesReturnInvalidPackages_ItShouldOnlyReturnTheApplicablePackages()
        {
            var solver = new Solver();

            var stringPipeline = A.Fake<IPipeExtension>();
            var int32Pipeline = A.Fake<IPipeExtension>();

            var stringOutputPackage = new PipeOutputPackage(typeof(string), typeof(string), o => "incorrect package");
            var int32OutputPackage = new PipeOutputPackage(typeof(int), typeof(string), o => "correct");

            A.CallTo(() => stringPipeline.PipeFrom(typeof(string))).Returns(new[] { stringOutputPackage });
            A.CallTo(() => int32Pipeline.PipeFrom(typeof(int))).Returns(new[] { int32OutputPackage });

            solver.Install(stringPipeline);
            solver.Install(int32Pipeline);

            var solvedPackage = solver.SolveAsPipePackage(typeof(int), typeof(string));
            object processedOutput = solvedPackage.ProcessInput(123);

            Assert.AreEqual("correct", processedOutput);
        }

        [Test]
        public void Install_WhenNullToPipeExtensionParameter_ItShouldThrowArgumentNullException()
        {
            var solver = new Solver();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => solver.Install(null));
            Assert.AreEqual("pipeExtension", argumentNullException.ParamName);
        }
    }
}