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
        public void PipeFrom_WhenPipelinesReturnInvalidDataTypes_ItShouldNotReturnTheInvalidPackages()
        {
            var registry = new SemanticRegistry();

            var stringPipeline = A.Fake<IPipeExtension>();
            var int32Pipeline = A.Fake<IPipeExtension>();

            var stringOutputPackage = new PipeOutputPackage(typeof (string), typeof (string), o => null);
            var int32OutputPackage = new PipeOutputPackage(typeof (int), typeof (string), o => null);

            A.CallTo(() => stringPipeline.PipeFrom(typeof (string))).Returns(new[] {stringOutputPackage});
            A.CallTo(() => int32Pipeline.PipeFrom(typeof(int))).Returns(new[] { int32OutputPackage });

            registry.Install(stringPipeline);
            registry.Install(int32Pipeline);

            IEnumerable<PipeOutputPackage> pipeOutputPackages = registry.PipeFrom(typeof (int));
            var outputPackageList = pipeOutputPackages.ToList();

            Assert.AreEqual(1, outputPackageList.Count, "Unexpected number of returns");
            Assert.AreSame(int32OutputPackage, outputPackageList[0]);
        }

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