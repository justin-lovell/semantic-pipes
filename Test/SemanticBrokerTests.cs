using System;
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
        public void GivenEmptyBroker_WhenResolvingNullSource_ItShouldThrowArgumentNullException()
        {
            // arrange
            var builder = new SemanticBuilder();
            ISemanticBroker broker = builder.CreateBroker();

            // act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => broker.On<string>(null));

            // assert
            Assert.AreEqual("source", argumentNullException.ParamName);
        }

        [Test]
        public void GivenBrokerWithPipeFromObjectAToObjectB_WhenResolving_ItShouldInstantiateThePipe()
        {
            // pre-arrangement
            var expectedOutputObject = new TestObjectB();

            // arrangement
            var builder = new SemanticBuilder();
            builder.InstallPipe<TestObjectA, TestObjectB>(a => expectedOutputObject);

            ISemanticBroker broker = builder.CreateBroker();

            // act
            var actualOutputObject = broker.On(new TestObjectA()).Output<TestObjectB>();

            // assert
            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }
    }
}