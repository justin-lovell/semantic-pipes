using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_AutoMappingCollectionToCollection_Tests
    {
        private sealed class TestObjectA
        {
        }

        private sealed class TestObjectB
        {
        }

        [Test]
        public void GivenMappingFromObjectAToObjectB_WhenSolvingCollectionToCollection_ItShouldResolve()
        {
            // pre-arrangement
            int counter = 0;
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>((source, innerBroker) =>
            {
                counter++;
                return expectedReturnObject;
            });

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(new[] {new TestObjectA()}.AsEnumerable())
                .Output<IEnumerable<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
            Assert.AreEqual(1, counter);
        }
    }
}