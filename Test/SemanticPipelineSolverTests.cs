using System;
using FakeItEasy;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticPipelineSolverTests
    {
        [TestCase(typeof (string), typeof (int))]
        [TestCase(typeof (int), typeof (string))]
        public void SolveAsPipePackage_WhenTheInputAndOutputPairCannotBeResolved_ItShouldThrowNotImplementedExcpetion(
            Type inputType, Type outputType)
        {
            var registry = A.Fake<ISemanticRegistry>();
            // the pipe from below is deliberate as to try ensure no confusion goes through
            A.CallTo(() => registry.PipeFrom(typeof (string))).Returns(null);

            var solver = new SemanticPipelineSolver();
            solver.AppendRegistry(registry);

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

            var registry = A.Fake<ISemanticRegistry>();
            A.CallTo(() => registry.PipeFrom(solveToInputType)).Returns(new[] { expectedPackage });

            var solver = new SemanticPipelineSolver();
            solver.AppendRegistry(registry);

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
            var solver = new SemanticPipelineSolver();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => solver.AppendRegistry(null));

            Assert.AreEqual("registry", argumentNullException.ParamName);
        }

        [Test]
        public void AppendRegistry_WhenValidInstanceIsPassedIn_ItShouldAcceptIt()
        {
            var solver = new SemanticPipelineSolver();
            var registry = A.Fake<ISemanticRegistry>();

            A.CallTo(() => registry.PipeFrom(typeof (string))).Returns(null);

            solver.AppendRegistry(registry);
        }

        [Test]
        public void SolveAsPipePackage_WhenNoRegistryHasBeenAppended_ItShouldThrowNotSupportedExcpetion()
        {
            var solver = new SemanticPipelineSolver();

            Assert.Throws<NotSupportedException>(() => solver.SolveAsPipePackage(typeof (string), typeof (string)));
        }

        [Test]
        public void SolveAsPipePackage_WhenNullToInputTypeParameter_ItShouldThrowArgumentNullException()
        {
            var solver = new SemanticPipelineSolver();

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => solver.SolveAsPipePackage(null, typeof (string)));

            Assert.AreEqual("inputType", argumentNullException.ParamName);
        }

        [Test]
        public void SolveAsPipePackage_WhenNullToOutputTypeParameter_ItShouldThrowArgumentNullException()
        {
            var solver = new SemanticPipelineSolver();

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

            var registry = A.Fake<ISemanticRegistry>();
            A.CallTo(() => registry.PipeFrom(inputType)).Returns(new[] {expectedPackage});

            var solver = new SemanticPipelineSolver();
            solver.AppendRegistry(registry);


            PipeOutputPackage solvedPackage = solver.SolveAsPipePackage(inputType, outputType);


            Assert.AreSame(expectedPackage, solvedPackage);
        }
    }
}