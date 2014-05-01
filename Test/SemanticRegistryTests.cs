using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticRegistryTests
    {
        [Test]
        public void Install_WhenNullToPipelineParameter_ItShouldThrowArgumentNullExcpetion()
        {
            var registry = new SemanticRegistry();

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => registry.Install(null));

            Assert.AreEqual("extension", argumentNullException.ParamName);
        }
    }
}