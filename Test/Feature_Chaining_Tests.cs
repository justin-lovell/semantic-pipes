using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task GivenRegistryWithMultiplePipes_WhenResolvingFromAToD_ItShouldChainAllThePipes()
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

                var enrollmentList = new List<Action>
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) =>
                    {
                        Assert.AreSame(instanceClassA, a);
                        return instanceClassB;
                    }),
                    () => semanticBuilder.InstallPipe<TestClassB, TestClassC>((b, innerBroker) =>
                    {
                        Assert.AreSame(instanceClassB, b);
                        return instanceClassC;
                    }),
                    () => semanticBuilder.InstallPipe<TestClassC, TestClassD>((c, innerBroker) =>
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
                var solvedExecution = await broker.On(instanceClassA).Output<TestClassD>();

                // assert
                Assert.AreEqual(instanceClassD, solvedExecution);
            }
        }

        [Test]
        public async Task GivenRegistryWithMultipleSources_WhenResolvingToSpecificType_ItShouldResolve()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceClassB = new TestClassB();
            var instanceClassC = new TestClassC();

            // arrange
            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe<TestClassA, TestClassB>((a, semanticBroker) => instanceClassB);
            semanaticBuilder.InstallPipe<TestClassA, TestClassC>((a, semanticBroker) => instanceClassC);

            ISemanticBroker broker = semanaticBuilder.CreateBroker();

            // act
            var solveToB = await broker.On(instanceClassA).Output<TestClassB>();
            var solveToC = await broker.On(instanceClassA).Output<TestClassC>();

            // assert
            Assert.AreEqual(instanceClassB, solveToB);
            Assert.AreEqual(instanceClassC, solveToC);
        }

        [Test]
        public async Task GivenRegistryWithOnePipes_WhenResolvingFromAToB_ItShouldCallIt()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceClassB = new TestClassB();

            // arrange
            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe<TestClassA, TestClassB>((a, semanticBroker) =>
            {
                Assert.AreSame(instanceClassA, a);
                return instanceClassB;
            });

            ISemanticBroker broker = semanaticBuilder.CreateBroker();

            // act
            var solvedExecution = await broker.On(instanceClassA).Output<TestClassB>();

            // assert
            Assert.AreEqual(instanceClassB, solvedExecution);
        }

        [Test]
        public void GivenRegistry_WhenResolvingToUnresolveableDestination_ItShouldThrowCannotResolveSemanticException()
        {
            var instanceClassA = new TestClassA();
            var instanceClassC = new TestClassC();

            // arrange
            var semanaticBuilder = new SemanticBuilder();
            semanaticBuilder.InstallPipe<TestClassA, TestClassB>((a, semanticBroker) => new TestClassB());
            semanaticBuilder.InstallPipe<TestClassB, TestClassA>((b, semanticBroker) => new TestClassA());

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