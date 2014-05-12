using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes.Scenarios
{
    [TestFixture]
    public class SemanticBroker_PipeToEnumerableTests
    {
        [Test]
        public void WhenRegisteringPipe_ItShouldRegisterPackage()
        {
            var expectedReturnObject = new TestObjectB();
            var semanticBuilder = new SemanticBuilder();

            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>(source => expectedReturnObject);

            var semanticBroker = semanticBuilder.CreateBroker();
            var enumerable = semanticBroker.On(new TestObjectA())
                .Output<IEnumerable<TestObjectB>>();

            var returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        private sealed class TestObjectA { }
        private sealed class TestObjectB { }
    }
}