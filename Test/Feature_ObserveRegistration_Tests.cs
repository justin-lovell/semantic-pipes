using System;
using System.Collections.Generic;
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

        private class TestRegistryObserver : ISemanticRegistryObserver
        {
            private readonly Func<PipeOutputPackage, IEnumerable<PipeOutputPackage>> _callbackFunc;

            public TestRegistryObserver(Func<PipeOutputPackage, IEnumerable<PipeOutputPackage>> callbackFunc)
            {
                _callbackFunc = callbackFunc;
            }

            public IEnumerable<PipeOutputPackage> PipePackageInstalled(PipeOutputPackage package)
            {
                return _callbackFunc(package);
            }
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
            var observer = new TestRegistryObserver(package =>
            {
                wasEventCalled = true;
                sourceType = package.InputType;
                destinationType = package.OutputType;
                return null;
            });
            semanticBuilder.RegisterObserver(observer);

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
            var observer = new TestRegistryObserver(package =>
            {
                wasEventCalled = true;
                return null;
            });
            semanticBuilder.RegisterObserver(observer);

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
            
            var observer1 = new TestRegistryObserver(package =>
            {
                countPreRegisteredCalled++;
                return null;
            });
            var observer2 = new TestRegistryObserver(package =>
            {
                countPostRegisteredCalled++;
                return null;
            });

            // act
            semanticBuilder.RegisterObserver(observer1);
            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);
            semanticBuilder.RegisterObserver(observer2);

            // assert
            Assert.AreEqual(1, countPreRegisteredCalled);
            Assert.AreEqual(1, countPostRegisteredCalled);
        }
    }
}