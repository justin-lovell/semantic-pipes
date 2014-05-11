using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBuilderTests
    {
        private class TestClassA
        {
        }

        private class TestClassB
        {
        }

        private class TestClassC
        {
        }

        [Test]
        public void InstallPipe_WhenNullToProcessCallbackParameter_ItShouldThrowArgumentNullException()
        {
            var semanticBuilder = new SemanticBuilder();

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => semanticBuilder.InstallPipe<string, string>(null));

            Assert.AreEqual("processCallback", argumentNullException.ParamName);
        }

        [Test]
        public void InstallPipe_WhenTheProcessDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            Func<TestClassA, TestClassB> processCallback = a =>
            {
                Assert.AreSame(expectedTestClassA, a);
                return expectedTestClassB;
            };

            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe(processCallback);
            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            var processedOuput = semanticBroker.On(expectedTestClassA).Output<TestClassB>();

            Assert.AreSame(expectedTestClassB, processedOuput);
        }

        [Test]
        public void
            InstallPipe_WhenTheSameSourceDestinationTypesAreRegisteredMoreThanOnce_ItShouldThrowInvalidRegistryConfigurationException
            ()
        {
            var semanticBuilder = new SemanticBuilder();

            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);
            semanticBuilder.InstallPipe<TestClassA, TestClassC>(a => null);
            semanticBuilder.InstallPipe<TestClassB, TestClassA>(b => null);

            Assert.Throws<InvalidRegistryConfigurationException>(
                () => semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null));
        }

        [Test]
        public void PipeInstalled_WhenPipesAreAddedToInstallPipe_ItShouldOnlyRaiseTheRelevantEvent()
        {
            bool wasProcessCalled = false;

            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.PipeInstalled +=
                (sender, args) => args.PipeExtension.ProcessCallback(new TestClassA());

            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a =>
            {
                wasProcessCalled = true;
                return new TestClassB();
            });

            Assert.IsTrue(wasProcessCalled);
        }

        [Test]
        public void PipeInstalled_WhenPipesAreAddedToInstallPipe_ItShouldRaiseThePipeInstalledEvent()
        {
            bool wasEventCalled = false;

            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.PipeInstalled += (sender, args) => wasEventCalled = true;

            semanticBuilder.InstallPipe<TestClassA, TestClassB>(a => null);

            Assert.IsTrue(wasEventCalled);
        }
    }
}