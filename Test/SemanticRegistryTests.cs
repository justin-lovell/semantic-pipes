using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticRegistryTests
    {
        [Test]
        public void Install_WhenInstallPipeline_ItShouldAccept()
        {
            var extension = A.Fake<IPipeExtension>();
            var registry = new SemanticRegistry();

            registry.Install(extension);
        }

        [Test]
        public void Install_WhenNullToPipelineParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var registry = new SemanticRegistry();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => registry.Install(null));

            Assert.AreEqual("extension", argumentNullException.ParamName);
        }

        [Test]
        public void PipeFrom_WhenNullIsPassedIntoSourceTypeParmaeter_ItShouldThrowArgumentNullException()
        {
            var registry = new SemanticRegistry();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => registry.PipeFrom(null));

            Assert.AreEqual("sourceType", argumentNullException.ParamName);
        }
    }
}