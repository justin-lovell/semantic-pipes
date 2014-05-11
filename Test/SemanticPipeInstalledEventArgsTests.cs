using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticPipeInstalledEventArgsTests
    {
        [Test]
        public void AppendPackage_WhenCalled_ItShouldCallTheCallbackFromCtor()
        {
            PipeOutputPackage calledPackage = null;
            var args = new SemanticPipeInstalledEventArgs(p => calledPackage = p, null);
            var expectedPackage = new PipeOutputPackage(typeof(string), typeof(int), o => null);

            args.AppendPackage(expectedPackage);

            Assert.AreEqual(expectedPackage, calledPackage);
        }

        [Test]
        public void AppendPackage_WhenNullToPackageParameter_ItShouldThrowArgumentNullException()
        {
            var args = new SemanticPipeInstalledEventArgs(null, null);

            var argumentNullException = Assert.Throws<ArgumentNullException>(() => args.AppendPackage(null));

            Assert.AreEqual("package", argumentNullException.ParamName);
        }
    }
}