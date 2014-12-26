﻿using System.Collections.Generic;
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

        [Test]
        public void GivenTwoClasses_WhenSolvingSingleClassToArray_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>(source => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(new TestObjectA())
                .Output<TestObjectB[]>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public void GivenTwoClasses_WhenSolvingSingleClassToList_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectB();

            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB>(source => expectedReturnObject);

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(new TestObjectA())
                .Output<List<TestObjectB>>();


            // assert
            TestObjectB returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public void GivenAtoListB_WhenSolvingSingleAToListA_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectA();


            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, List<TestObjectB>>(
                source => new List<TestObjectB> {new TestObjectB()});

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(expectedReturnObject)
                .Output<List<TestObjectA>>();


            // assert
            var returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }

        [Test]
        public void GivenAtoArrayB_WhenSolvingSingleAToArraytA_ItShouldResolve()
        {
            // pre-arrangement
            var expectedReturnObject = new TestObjectA();


            // arrange
            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.InstallPipe<TestObjectA, TestObjectB[]>(source => new[] {new TestObjectB()});

            ISemanticBroker semanticBroker = semanticBuilder.CreateBroker();

            // act
            var enumerable = semanticBroker.On(expectedReturnObject)
                .Output<TestObjectA[]>();


            // assert
            var returnedType = enumerable.Single();
            Assert.AreSame(expectedReturnObject, returnedType);
        }
    }
}