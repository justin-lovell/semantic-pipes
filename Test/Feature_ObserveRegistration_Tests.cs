using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_ObserveRegistration_Tests
    {
        private class TestClassA
        {
        }

        private class TestClassB
        {
        }

        [Test]
        public void GivenPopulatedRegistry_WhenObserverEnrollsLater_ItShouldBeNotifiedOfPreviousRegistrations()
        {
            // pre-arrange
            bool wasEventCalled = false;
            Type sourceType = null;
            Type destinationType = null;

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);

            // act
            semanticBuilder.PipeInstalled += (sender, args) =>
            {
                wasEventCalled = true;
                sourceType = args.PipeExtension.SourceType;
                destinationType = args.PipeExtension.DestinationType;
            };

            // assert
            Assert.IsTrue(wasEventCalled);
            Assert.AreEqual(typeof (TestClassA), sourceType);
            Assert.AreEqual(typeof (TestClassB), destinationType);
        }

        [Test]
        public void GivenRegistry_WhenPipesAreRegistered_ItShouldNotifyObservers()
        {
            // pre-arrange
            bool wasEventCalled = false;

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.PipeInstalled += (sender, args) => wasEventCalled = true;

            // act
            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);

            // assert
            Assert.IsTrue(wasEventCalled);
        }

        [Test]
        public void GivenRegistry_WhenObserverIsInserted_ItShouldDetectPriorAndAfterRegistrations()
        {
            // pre-arrange
            int countPreRegisteredCalled = 0;
            int countPostRegisteredCalled = 0;

            // arrange
            var semanticBuilder = new SemanticBuilder();

            // act
            semanticBuilder.PipeInstalled += (sender, args) => countPreRegisteredCalled++;
            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);
            semanticBuilder.PipeInstalled += (sender, args) => countPostRegisteredCalled++;

            // assert
            Assert.AreEqual(1, countPreRegisteredCalled);
            Assert.AreEqual(1, countPostRegisteredCalled);
        }
    }
}