using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_AutoMappingObjectsToCollectionObjects_Tests
    {
        private sealed class TestObjectA
        {
        }

        private sealed class TestObjectB
        {
        }

        [Test]
        [Ignore]
        public void GivenTwoClasses_WhenSolvingSingleClassToCollection_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>(source => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(new TestObjectA())
                .Output<IEnumerable<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }
    }
}