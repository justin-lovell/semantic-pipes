using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_ContravarianceWalking_Tests
    {
        private class TestClassA
        {
        }

        private class TestClassB
        {
        }

        private class TestSuperClassB : TestClassB, ITestInterface
        {
        }

        private interface ITestInterface
        {
        }

        [Test]
        public async Task GivenPipeOnSuperClass_WhenResolvingToBaseType_ItShouldReturnSuperType()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceSuperClassB = new TestSuperClassB();

            // arrange
            var builder = new SemanticBuilder();

            builder.InstallPipe<TestClassA, IEnumerable<TestSuperClassB>>(
                (@as, broker) =>
                {
                    Assert.AreSame(instanceClassA, @as);
                    return new[] {instanceSuperClassB};
                });

            var semanticBroker = builder.CreateBroker();

            // act
            var returnedInstance = await semanticBroker.On(instanceClassA).Output<IEnumerable<TestClassB>>();

            // assert
            var result = returnedInstance.Single();
            Assert.AreSame(instanceSuperClassB, result);
        }

        [Test]
        public async Task GivenPipeOnSuperClass_WhenResolvingToInterface_ItShouldReturnSuperType()
        {
            // pre-arrange
            var instanceClassA = new TestClassA();
            var instanceSuperClassB = new TestSuperClassB();

            // arrange
            var builder = new SemanticBuilder();

            builder.InstallPipe<TestClassA, IEnumerable<TestSuperClassB>>(
                (@as, broker) =>
                {
                    Assert.AreSame(instanceClassA, @as);
                    return new[] { instanceSuperClassB };
                });

            var semanticBroker = builder.CreateBroker();

            // act
            var returnedInstance = await semanticBroker.On(instanceClassA).Output<IEnumerable<ITestInterface>>();

            // assert
            var result = returnedInstance.Single();
            Assert.AreSame(instanceSuperClassB, result);
        }
    }
}