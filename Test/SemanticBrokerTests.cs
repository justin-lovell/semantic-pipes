using System;
using FakeItEasy;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBrokerTests
    {
        private class TestObjectA
        {
        }

        private class TestObjectB
        {
        }

        [Test]
        public void On_WhenNullToSourceParameter_ItShouldThrowArgumentNullException()
        {
            var broker = new SemanticBroker();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => broker.On<string>(null));
            Assert.AreEqual("source", argumentNullException.ParamName);
        }

        [Test]
        public void On_WhenUsingRegisteredPipelinesAndWeSearchForSolveableComponents_ItShouldTranslate()
        {
            var expectedOutputObject = new TestObjectB();

            Type inputType = typeof (TestObjectA);
            Type outputType = expectedOutputObject.GetType();


            var expectedPackage = new PipeOutputPackage(inputType, outputType, o => expectedOutputObject);

            var pipeExtension = A.Fake<IPipeExtension>();
            A.CallTo(() => pipeExtension.PipeFrom(inputType)).Returns(new[] {expectedPackage});

            var broker = new SemanticBroker();
            broker.Install(pipeExtension);


            var actualOutputObject = broker.On(new TestObjectA()).Output<TestObjectB>();


            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }
    }
}