using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class SemanticBuilderTests
    {
        private class TestClassA
        {
        }

        private class TestClassB
        {
        }

        [Test]
        public void Pipe_WhenTheProcessDelegateIsCalled_ItShouldExecuteTheSpecifiedGenericCallback()
        {
            var expectedTestClassA = new TestClassA();
            var expectedTestClassB = new TestClassB();

            Func<TestClassA, TestClassB> processCallback = a =>
            {
                Assert.AreSame(expectedTestClassA, a);
                return expectedTestClassB;
            };

            var semanticBuilder = new SemanticBuilder();
            semanticBuilder.Pipe(processCallback);
            var semanticBroker = semanticBuilder.CreateBroker();

            var processedOuput = semanticBroker.On(expectedTestClassA).Output<TestClassB>();

            Assert.AreSame(expectedTestClassB, processedOuput);
        }

        [Test]
        public void Pipe_WhenNullToProcessCallbackParameter_ItShouldThrowArgumentNullException()
        {
            var semanticBuilder = new SemanticBuilder();

            var argumentNullException =
                Assert.Throws<ArgumentNullException>(() => semanticBuilder.Pipe<string, string>(null));

            Assert.AreEqual("processCallback", argumentNullException.ParamName);
        }
    }
}