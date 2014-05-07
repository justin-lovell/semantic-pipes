using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class GenericPipeExtensionTests
    {
        [Test]
        public void Ctor_WhenNullToProcessCallbackParameter_ItShouldThrowArgumentNullException()
        {
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new GenericPipeExtension<TestClassA, TestClassB>(null));

            Assert.AreEqual("processCallback", argumentNullException.ParamName);
        }

        [Test]
        public void PipeFrom_WhenNullToSourceTypeParameter_ItShouldReturnNull()
        {
            var extension = new GenericPipeExtension<TestClassA, TestClassB>(a => null);

            IEnumerable<PipeOutputPackage> pipeOutputPackages = extension.PipeFrom(null);

            Assert.IsNull(pipeOutputPackages);
        }

        [Test]
        public void PipeFrom_WhenUninterestedSourceTypeQueried_ItShouldReturnNull()
        {
            Type queryType = typeof(TestClassB);
            var extension = new GenericPipeExtension<TestClassA, TestClassA>(a => null);

            IEnumerable<PipeOutputPackage> pipeOutputPackages = extension.PipeFrom(queryType);

            Assert.IsNull(pipeOutputPackages);
        }

        [Test]
        public void PipeFrom_WhenInterestedForSourceTypeQueried_ItShouldReturnValidPipeOutPackage()
        {
            Type queryType = typeof(TestClassA);
            var extension = new GenericPipeExtension<TestClassA, TestClassB>(a => null);

            IEnumerable<PipeOutputPackage> pipeOutputPackages = extension.PipeFrom(queryType);

            Assert.IsNotNull(pipeOutputPackages);

            var package = pipeOutputPackages.Single();

            Assert.AreEqual(typeof(TestClassB), package.OutputType);
        }

        [Test]
        public void PipeFrom_WhenTheProcessDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            Func<TestClassA, TestClassB> processCallback = a =>
            {
                Assert.AreSame(expectedTestClassA, a);
                return expectedTestClassB;
            };
            var extension = PipeExtensionFactory.Process(processCallback);

            IEnumerable<PipeOutputPackage> pipeOutputPackages = extension.PipeFrom(typeof(TestClassA));
            var package = pipeOutputPackages.Single();

            object processedInput = package.ProcessInput(expectedTestClassA);

            Assert.AreSame(expectedTestClassB, processedInput);
        }

        private class TestClassA { }
        private class TestClassB { }
    }
}