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
                var pipePackageOption = new PipeOutputPackage(null, typeof(string), processCallbackFunc);

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
                var pipePackageOption = new PipeOutputPackage(typeof(string), null, processCallbackFunc);

                Assert.Fail();
            });

            Assert.AreEqual("outputType", argumentNullException.ParamName);
        }

        [Test]
        public void Ctor_WhenNullToProcessCallbackFuncParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                var pipePackageOption = new PipeOutputPackage(typeof(string), typeof (string), null);
                Assert.Fail();
            });

            Assert.AreEqual("processCallbackFunc", argumentNullException.ParamName);
        }

        [Test]
        public void ProcessInput_WhenNullToInputParameter_ItShouldThrowArgumentNullException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = new PipeOutputPackage(typeof(string), typeof (string), processCallbackFunc);

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => pipePackageOption.ProcessInput(null));

            Assert.AreEqual("input", argumentNullException.ParamName);
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackEchoesInput_ItShouldReturnTheInput()
        {
            Func<object, object> processCallbackFunc = input => input;
            var pipePackageOption = new PipeOutputPackage(typeof(string), typeof (string), processCallbackFunc);

            var output = pipePackageOption.ProcessInput("abc");

            Assert.AreEqual("abc", output);
        }

        [Test]
        public void ProcessInput_WhenInputParameterIsNotTheExpectedType_ItShouldThrowNotArgumentException()
        {
            Func<object, object> processCallbackFunc = input => "abc";
            var pipePackageOption = new PipeOutputPackage(typeof(int), typeof(string), processCallbackFunc);

            var argumentException = Assert.Throws<ArgumentException>(() => pipePackageOption.ProcessInput("abc"));
            Assert.AreEqual("input", argumentException.ParamName);
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackReturnsWhatIsNotTheExpectedOutputType_ItShouldThrowNotSupportedException()
        {
            Func<object, object> processCallbackFunc = input => 2;
            var pipePackageOption = new PipeOutputPackage(typeof(string), typeof(string), processCallbackFunc);

            Assert.Throws<NotSupportedException>(() => pipePackageOption.ProcessInput("abc"));
        }

        [Test]
        public void ProcessInput_WhenProcesCallbackReturnsNull_ItShouldThrowNotSupportedException()
        {
            Func<object, object> processCallbackFunc = input => null;
            var pipePackageOption = new PipeOutputPackage(typeof(string), typeof (string), processCallbackFunc);

            Assert.Throws<NotSupportedException>(() => pipePackageOption.ProcessInput("abc"));
        }
    }
}