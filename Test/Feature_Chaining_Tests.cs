using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_Chaining_Tests
    {
        private class TestClassA
        {
        }

        private class TestClassB
        {
        }

        private class TestClassC
        {
        }

        private class TestClassD
        {
        }

        [Test]
        public void GivenRegistryWithMultiplePipes_WhenResolvingFromAToD_ItShouldChainAllThePipes()
        {
            for (int counter = 0; counter < 15; counter++)
            {
                // pre-arrange
                var instanceClassA = new TestClassA();
                var instanceClassB = new TestClassB();
                var instanceClassC = new TestClassC();
                var instanceClassD = new TestClassD();

                // arrange
                var semanticBuilder = new SemanticBuilder();

                var enrollmentList = new List<Action>()
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>(a =>
                    {
                        Assert.AreSame(instanceClassA, a);
                        return instanceClassB;
                    }),
                    () => semanticBuilder.InstallPipe<TestClassB, TestClassC>(b =>
                    {
                        Assert.AreSame(instanceClassB, b);
                        return instanceClassC;
                    }),
                    () => semanticBuilder.InstallPipe<TestClassC, TestClassD>(c =>
                    {
                        Assert.AreSame(instanceClassC, c);
                        return instanceClassD;
                    }),
                };

                var enrollmentActions = enrollmentList.OrderBy(x=>Guid.NewGuid());
                foreach (var enrollmentAction in enrollmentActions)
                {
                    enrollmentAction();
                }

                ISemanticBroker broker = semanticBuilder.CreateBroker();

                // act
                var solvedExecution = broker.On(instanceClassA).Output<TestClassD>();

                // assert
                Assert.AreEqual(instanceClassD, solvedExecution);
            }
        }

        [Test]
        public void GivenRegistryWithMultipleSources_WhenResolvingToSpecificType_ItShouldResolve()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceClassB = new TestClassB();
            var instanceClassC = new TestClassC();

            // arrange
            Func<TestClassA, TestClassB> aToB = a => instanceClassB;
            Func<TestClassA, TestClassC> aToC = a => instanceClassC;


            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe(aToB);
            semanaticBuilder.InstallPipe(aToC);

            ISemanticBroker broker = semanaticBuilder.CreateBroker();

            // act
            var solveToB = broker.On(instanceClassA).Output<TestClassB>();
            var solveToC = broker.On(instanceClassA).Output<TestClassC>();

            // assert
            Assert.AreEqual(instanceClassB, solveToB);
            Assert.AreEqual(instanceClassC, solveToC);
        }

        [Test]
        public void GivenRegistryWithOnePipes_WhenResolvingFromAToB_ItShouldCallIt()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceClassB = new TestClassB();

            // arrange
            Func<TestClassA, TestClassB> aToB = a =>
            {
                Assert.AreSame(instanceClassA, a);
                return instanceClassB;
            };


            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe(aToB);

            ISemanticBroker broker = semanaticBuilder.CreateBroker();

            // act
            var solvedExecution = broker.On(instanceClassA).Output<TestClassB>();

            // assert
            Assert.AreEqual(instanceClassB, solvedExecution);
        }

        [Test]
        public void GivenRegistry_WhenResolvingToUnresolveableDestination_ItShouldThrowCannotResolveSemanticException()
        {
            var instanceClassA = new TestClassA();
            var instanceClassC = new TestClassC();

            // arrange
            Func<TestClassA, TestClassB> aToB = a => new TestClassB();
            Func<TestClassB, TestClassA> bToA = a => new TestClassA();

            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe(aToB);
            semanaticBuilder.InstallPipe(bToA);

            ISemanticBroker broker = semanaticBuilder.CreateBroker();

            // act
            var exception1 =
                Assert.Throws<CannotResolveSemanticException>(() => broker.On(instanceClassA).Output<TestClassC>());
            var exception2 =
                Assert.Throws<CannotResolveSemanticException>(() => broker.On(instanceClassC).Output<TestClassA>());

            // assert
            string expectedExceptionMessage1 =
                string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                    typeof (TestClassA), typeof (TestClassC));
            string expectedExceptionMessage2 =
                string.Format("The input type '{0}' could not be resolved to output a type of {1}",
                    typeof (TestClassC), typeof (TestClassA));

            Assert.AreEqual(expectedExceptionMessage1, exception1.Message);
            Assert.AreEqual(expectedExceptionMessage2, exception2.Message);
        }
    }
}