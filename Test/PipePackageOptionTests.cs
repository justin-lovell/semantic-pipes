using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class PipePackageOptionTests
    {
        [Test]
        public void Ctor_WhenNullToOutputTypeParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                Func<object, object> processCallbackFunc = input => null;
                var pipePackageOption = new PipePackageOption(null, processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("outputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToProcessCallbackFuncParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                var pipePackageOption = new PipePackageOption(typeof (string), null);
                Assert.Fail();
            });

            Assert.AreEqual("processCallbackFunc", argumentNullException.ParamName);
        }

        [Test]
        public void ProcessInput_WhenNullToInputParameter_ItShouldThrowArgumentNullException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = new PipePackageOption(typeof (string), processCallbackFunc);

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => pipePackageOption.ProcessInput(null));

            Assert.AreEqual("input", argumentNullException.ParamName);
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackEchoesInput_ItShouldReturnTheInput()
        {
            Func<object, object> processCallbackFunc = input => input;
            var pipePackageOption = new PipePackageOption(typeof (string), processCallbackFunc);

            var output = pipePackageOption.ProcessInput("abc");

            Assert.AreEqual("abc", output);
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackReturnsWhatIsNotTheExpectedOutputType_ItShouldThrowNotSupportedException()
        {
            Func<object, object> processCallbackFunc = input => 2;
            var pipePackageOption = new PipePackageOption(typeof(string), processCallbackFunc);

            Assert.Throws<NotSupportedException>(() => pipePackageOption.ProcessInput("abc"));
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackReturnsNull_ItShouldThrowNotSupportedException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = new PipePackageOption(typeof (string), processCallbackFunc);

            Assert.Throws<NotSupportedException>(() => pipePackageOption.ProcessInput("abc"));
        }
    }
}