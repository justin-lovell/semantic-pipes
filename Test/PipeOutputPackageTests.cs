using System;
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
                Func<object, object> processCallbackFunc = input => null;
                var pipePackageOption = PipeOutputPackage.Direct(null, typeof(string), processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("inputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToOutputTypeParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                Func<object, object> processCallbackFunc = input => null;
                var pipePackageOption = PipeOutputPackage.Direct(typeof(string), null, processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("outputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToProcessCallbackFuncParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                var pipePackageOption = PipeOutputPackage.Direct(typeof(string), typeof(string), null);
                Assert.Fail();
            });

            Assert.AreEqual("processCallbackFunc", argumentNullException.ParamName);
        }

        [Test]
        public void GivenPackageWhichProcessesIntegers_WhenInputParameterIsStringType_ItShouldThrowNotArgumentException()
        {
            Func<object, object> processCallbackFunc = input => "abc";
            var pipePackageOption = PipeOutputPackage.Direct(typeof(int), typeof(string), processCallbackFunc);

            var argumentException = Assert.Throws<ArgumentException>(() => pipePackageOption.ProcessInput("abc"));
            Assert.AreEqual("input", argumentException.ParamName);
        }

        [Test]
        public void GivenPackageInstance_WhenNullPassedIntoInputParameter_ItShouldThrowArgumentNullException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = PipeOutputPackage.Direct(typeof(string), typeof(string), processCallbackFunc);

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => pipePackageOption.ProcessInput(null));

            Assert.AreEqual("input", argumentNullException.ParamName);
        }

        [Test]
        public void GivenEchoCallback_WhenItIsCalledWithInput_ItShouldReturnTheInput()
        {
            Func<object, object> processCallbackFunc = input => input;
            var pipePackageOption = PipeOutputPackage.Direct(typeof(string), typeof(string), processCallbackFunc);

            object output = pipePackageOption.ProcessInput("abc");

            Assert.AreEqual("abc", output);
        }

        [Test]
        public void GivenCallbackWhichReturnsNull_WhenLegallyCalled_ItShouldThrowUnexpectedPipePackageOperationException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = PipeOutputPackage.Direct(typeof(string), typeof(string), processCallbackFunc);

            Assert.Throws<UnexpectedPipePackageOperationException>(() => pipePackageOption.ProcessInput("abc"));
        }

        [Test]
        public void
            GivenProcessorWhichReturnsString_WhenProcesCallbackReturnsInteger_ItShouldThrowUnexpectedPipePackageOperationException
            ()
        {
            Func<object, object> processCallbackFunc = input => 2;
            var pipePackageOption = PipeOutputPackage.Direct(typeof(string), typeof(string), processCallbackFunc);

            Assert.Throws<UnexpectedPipePackageOperationException>(() => pipePackageOption.ProcessInput("abc"));
        }
    }
}