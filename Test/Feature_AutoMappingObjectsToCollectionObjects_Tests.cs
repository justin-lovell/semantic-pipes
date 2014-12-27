using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task GivenAtoArrayB_WhenSolvingSingleAToArraytA_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectA();


            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB[]>((source, innerBroker) => new[] {new TestObjectB()});

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(expectedReturnObject).Output<TestObjectA[]>();


            // assert
            TestObjectA returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public async Task GivenAtoListB_WhenSolvingSingleAToListA_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectA();


            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, List<TestObjectB>>(
                (source, innerBroker) => new List<TestObjectB> {new TestObjectB()});

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(expectedReturnObject).Output<List<TestObjectA>>();


            // assert
            TestObjectA returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public async Task GivenTwoClasses_WhenSolvingSingleClassToArray_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>((source, innerBroker) => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(new TestObjectA()).Output<TestObjectB[]>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public async Task GivenTwoClasses_WhenSolvingSingleClassToEnumerable_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>((source, innerBroker) => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(new TestObjectA()).Output<IEnumerable<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public async Task GivenTwoClasses_WhenSolvingSingleClassToList_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>((source, innerBroker) => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(new TestObjectA()).Output<List<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public async Task GivenTwoClassesWhenRegisteredAsList_WhenSolvingSingleClassToEnumerable_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, List<TestObjectB>>(
                (source, innerBroker) => new List<TestObjectB> {expectedReturnObject});

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = await semanticBroker.On(new TestObjectA()).Output<IEnumerable<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }
    }
}