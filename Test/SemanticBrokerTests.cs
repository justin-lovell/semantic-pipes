using System;
using System.Threading.Tasks;
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
        public async Task GivenBrokerWithPipeFromObjectAToObjectB_WhenResolving_ItShouldInstantiateThePipe()
        {
            // pre-arrangement
            var expectedOutputObject = new TestClassB();

            // arrangement
            var builder = new SemanticBuilder();
            builder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) => expectedOutputObject);

            ISemanticBroker broker = builder.CreateBroker();

            // act
            var actualOutputObject = await broker.On(new TestClassA()).Output<TestClassB>();

            // assert
            Assert.AreSame(expectedOutputObject, actualOutputObject);
        }


        [Test]
        public async Task GivenRegistration_WhenTheProcessDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            // pre-arrangement
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            // arrange
            var semanticBuilder = new SemanticBuilder();

            semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) =>
            {
                Assert.AreSame(expectedTestClassA, a);
                return expectedTestClassB;
            });
            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var processedOuput = await semanticBroker.On(expectedTestClassA).Output<TestClassB>();

            // assert
            Assert.AreSame(expectedTestClassB, processedOuput);
        }


        [Test]
        public async Task GivenRegistration_WhenTheProcessAsyncDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            // pre-arrangement
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            // arrange
            var semanticBuilder = new SemanticBuilder();

            semanticBuilder.InstallPipe<TestClassA, TestClassB>(async (a, innerBroker) =>
            {
                Assert.AreSame(expectedTestClassA, a);

                await Task.Delay(TimeSpan.FromMilliseconds(100));
                return expectedTestClassB;
            });
            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var processedOuput = await semanticBroker.On(expectedTestClassA).Output<TestClassB>();

            // assert
            Assert.AreSame(expectedTestClassB, processedOuput);
        }
    }
}