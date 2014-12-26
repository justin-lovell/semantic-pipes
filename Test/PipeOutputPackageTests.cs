using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class PipeOutputPackageTests
    {
        [Test]
        public void Ctor_WhenNullToInputTypeParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                PipeCallback processCallbackFunc = (input, broker) => null;
                PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(null, typeof (string),
                    processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("inputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToOutputTypeParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                PipeCallback processCallbackFunc = (input, broker) => null;
                PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), null,
                    processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("outputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToProcessCallbackFuncParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), typeof (string), null);
                Assert.Fail();
            });

            Assert.AreEqual("processCallbackFunc", argumentNullException.ParamName);
        }

        [Test]
        public void GivenCallbackWhichReturnsNull_WhenLegallyCalled_ItShouldThrowUnexpectedPipePackageOperationException
            ()
        {
            PipeCallback processCallbackFunc = (input, broker) => null;
            PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), typeof (string),
                processCallbackFunc);

            Assert.Throws<UnexpectedPipePackageOperationException>(
                async () => await pipePackageOption.ProcessInput("abc", null));
        }

        [Test]
        public async Task GivenEchoCallback_WhenItIsCalledWithInput_ItShouldReturnTheInput()
        {
            PipeCallback processCallbackFunc = (input, broker) => Task.FromResult(input);
            PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), typeof (string),
                processCallbackFunc);

            object output = await pipePackageOption.ProcessInput("abc", null);

            Assert.AreEqual("abc", output);
        }

        [Test]
        public void GivenPackageInstance_WhenNullPassedIntoInputParameter_ItShouldThrowArgumentNullException()
        {
            PipeCallback processCallbackFunc = (input, broker) => null;
            PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), typeof (string),
                processCallbackFunc);

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(async () => await pipePackageOption.ProcessInput(null, null));

            Assert.AreEqual("input", argumentNullException.ParamName);
        }

        [Test]
        public void GivenPackageWhichProcessesIntegers_WhenInputParameterIsStringType_ItShouldThrowNotArgumentException()
        {
            PipeCallback processCallbackFunc = (input, broker) => Task.FromResult((object) "abc");
            PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (int), typeof (string),
                processCallbackFunc);

            var argumentException =
                Assert.Throws<ArgumentException>(async () => await pipePackageOption.ProcessInput("abc", null));
            Assert.AreEqual("input", argumentException.ParamName);
        }

        [Test]
        public void
            GivenProcessorWhichReturnsString_WhenProcesCallbackReturnsInteger_ItShouldThrowUnexpectedPipePackageOperationException
            ()
        {
            PipeCallback processCallbackFunc = (input, broker) => Task.FromResult((object) 2);
            PipeOutputPackage pipePackageOption = PipeOutputPackage.Direct(typeof (string), typeof (string),
                processCallbackFunc);

            Assert.Throws<UnexpectedPipePackageOperationException>(
                async () => await pipePackageOption.ProcessInput("abc", null));
        }
    }
}