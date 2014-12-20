using System;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBuilderTests
    {
        [Test]
        public void GivenAnInstance_WhenNullToProcessCallbackParameter_ItShouldThrowArgumentNullException()
        {
            // arrange
            var semanticBuilder = new SemanticBuilder();

            // act
            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => semanticBuilder.InstallPipe<string, string>(null));

            // assert
            Assert.AreEqual("processCallback", argumentNullException.ParamName);
        }
    }
}