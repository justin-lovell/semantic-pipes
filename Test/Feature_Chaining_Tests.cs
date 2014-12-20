using System;
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
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceClassB = new TestClassB();
            var instanceClassC = new TestClassC();
            var instanceClassD = new TestClassD();

            // arrange
            Func<TestClassA, TestClassB> aToB = a =>
            {
                Assert.AreSame(instanceClassA, a);
                return instanceClassB;
            };
            Func<TestClassB, TestClassC> bToC = b =>
            {
                Assert.AreSame(instanceClassB, b);
                return instanceClassC;
            };
            Func<TestClassC, TestClassD> cToD = c =>
            {
                Assert.AreSame(instanceClassC, c);
                return instanceClassD;
            };


            var semenaticBuilder = new SemanticBuilder();

            semenaticBuilder.InstallPipe(aToB);
            semenaticBuilder.InstallPipe(bToC);
            semenaticBuilder.InstallPipe(cToD);

            ISemanticBroker broker = semenaticBuilder.CreateBroker();

            // act
            var solvedExecution = broker.On(instanceClassA).Output<TestClassD>();

            // assert
            Assert.AreEqual(instanceClassD, solvedExecution);
        }
    }
}