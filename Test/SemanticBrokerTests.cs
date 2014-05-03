using System;
using FakeItEasy;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBrokerTests
    {
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

            var inputType = typeof (TestObjectA);
            var outputType = expectedOutputObject.GetType();


            var expectedPackage = new PipeOutputPackage(inputType, outputType, o => expectedOutputObject);

            var registry = A.Fake<ISemanticRegistry>();
            A.CallTo(() => registry.PipeFrom(inputType)).Returns(new[] { expectedPackage });

            var broker = new SemanticBroker();
            broker.AppendRegistry(registry);


            var actualOutputObject = broker.On(new TestObjectA()).Output<TestObjectB>();


            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }

        private class TestObjectA { }
        private class TestObjectB { }
    }
}