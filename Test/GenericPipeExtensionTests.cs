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

        private class TestClassA { }
        private class TestClassB { }
    }
}