using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBrokerTests
    {
        private class TestClassA
        {
        }

        private class TestClassB
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
            var expectedOutputObject = new TestClassB();

            // arrangement
            var builder = new SemanticBuilder();
            builder.InstallPipe<TestClassA, TestClassB>(a => expectedOutputObject);

            ISemanticBroker broker = builder.CreateBroker();

            // act
            var actualOutputObject = broker.On(new TestClassA()).Output<TestClassB>();

            // assert
            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }


        [Test]
        public void GivenRegistration_WhenTheProcessDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            // pre-arrangement
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            // arrange
            var semanticBuilder = new SemanticBuilder();

            Func<TestClassA, TestClassB> processCallback = a =>
            {
                Assert.AreSame(expectedTestClassA, a);
                return expectedTestClassB;
            };
            semanticBuilder.InstallPipe(processCallback);
            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var processedOuput = semanticBroker.On(expectedTestClassA).Output<TestClassB>();

            // assert
            Assert.AreSame(expectedTestClassB, processedOuput);
        }
    }
}