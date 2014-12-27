using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_RegistrationGuarding_Tests
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

        [Test]
        public void
            GivenPopulatedRegistry_WhenDuplicatingRegistrationMoreThanOnce_ItShouldThrowInvalidRegistryConfigurationException
            ()
        {
            // arrange
            var semanticBuilder = new SemanticBuilder();

            semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) => (TestClassB)null);
            semanticBuilder.InstallPipe<TestClassA, TestClassC>((a, innerBroker) => (TestClassC)null);
            semanticBuilder.InstallPipe<TestClassB, TestClassA>((b, innerBroker) => (TestClassA)null);

            // act
            Assert.Throws<InvalidRegistryConfigurationException>(
                () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) => (TestClassB)null));
        }
    }
}