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
            var builder = new SemanticBuilder();
            var broker = builder.CreateBroker();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => broker.On<string>(null));
            Assert.AreEqual("source", argumentNullException.ParamName);
        }

        [Test]
        public void On_WhenUsingRegisteredPipelinesAndWeSearchForSolveableComponents_ItShouldTranslate()
        {
            var expectedOutputObject = new TestObjectB();

            var builder = new SemanticBuilder();
            builder.InstallPipe<TestObjectA, TestObjectB>(a => expectedOutputObject);

            var broker = builder.CreateBroker();


            var actualOutputObject = broker.On(new TestObjectA()).Output<TestObjectB>();


            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }
    }
}