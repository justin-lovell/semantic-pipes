﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SemanticPipes
{
    [TestFixture]
    public class Feature_ShortCircuiting_Tests
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
        public async Task GivenRegistryWithMultiplePipes_WhenResolvingFromAToD_ItShouldUseTheDirectShortCircuit()
        {
            for (int counter = 0; counter < 15; counter++)
            {
                // pre-arrange
                var instanceClassA = new TestClassA();
                var instanceClassD = new TestClassD();

                // arrange
                var semanticBuilder = new SemanticBuilder();

                var enrollmentList = new List<Action>
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) => new TestClassB()),
                    () => semanticBuilder.InstallPipe<TestClassB, TestClassC>((b, innerBroker) => new TestClassC()),
                    () => semanticBuilder.InstallPipe<TestClassC, TestClassD>((c, innerBroker) => new TestClassD()),
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassD>((a, innerBroker) =>
                    {
                        Assert.AreSame(instanceClassA, a);
                        return instanceClassD;
                    })
                };

                var enrollmentActions = enrollmentList.OrderBy(x => Guid.NewGuid());
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
        public async Task GivenRegistryWithMultiplePipes_WhenResolvingFromAToD_ItShouldUseThePartialShortCircuit()
        {
            for (int counter = 0; counter < 15; counter++)
            {
                // pre-arrange
                var instanceClassA = new TestClassA();
                var instanceClassC = new TestClassC();
                var instanceClassD = new TestClassD();

                // arrange
                var semanticBuilder = new SemanticBuilder();

                var enrollmentList = new List<Action>
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) => new TestClassB()),
                    () => semanticBuilder.InstallPipe<TestClassB, TestClassC>((b, innerBroker) => new TestClassC()),
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassC>((a, innerBroker) =>
                    {
                        Assert.AreSame(instanceClassA, a);
                        return instanceClassC;
                    }),
                    () => semanticBuilder.InstallPipe<TestClassC, TestClassD>((c, innerBroker) =>
                    {
                        Assert.AreSame(instanceClassC, c);
                        return instanceClassD;
                    })
                };

                var enrollmentActions = enrollmentList.OrderBy(x => Guid.NewGuid());
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
        public async Task GivenRegistryWithPipe_WhenResolvingFromEnumerableToEnumerable_ItShouldUseTheShortCircuit()
        {
            for (int counter = 0; counter < 15; counter++)
            {
                // arrange
                var semanticBuilder = new SemanticBuilder();

                var enrollmentList = new List<Action>
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) =>
                    {
                        Assert.Fail();
                        return new TestClassB();
                    }),
                    () => semanticBuilder.InstallPipe<IEnumerable<TestClassA>, IEnumerable<TestClassB>>(
                        (a, innerBroker) => from b in a select new TestClassB())
                };

                var enrollmentActions = enrollmentList.OrderBy(x => Guid.NewGuid());
                foreach (var enrollmentAction in enrollmentActions)
                {
                    enrollmentAction();
                }

                ISemanticBroker broker = semanticBuilder.CreateBroker();

                // act
                await broker.On(new[] { new TestClassA() }).Output<TestClassB[]>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<TestClassB[]>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<List<TestClassB>>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<IEnumerable<TestClassB>>();
                await broker.On(new[] { new TestClassA() }).Output<List<TestClassB>>();
            }
        }


        [Test]
        public async Task GivenRegistryWithPipe_WhenResolvingFromListToArray_ItShouldUseTheShortCircuit()
        {
            for (int counter = 0; counter < 15; counter++)
            {
                // arrange
                var semanticBuilder = new SemanticBuilder();

                var enrollmentList = new List<Action>
                {
                    () => semanticBuilder.InstallPipe<TestClassA, TestClassB>((a, innerBroker) =>
                    {
                        Assert.Fail();
                        return new TestClassB();
                    }),
                    () => semanticBuilder.InstallPipe<List<TestClassA>, TestClassB[]>(
                        (a, innerBroker) => (from b in a select new TestClassB()).ToArray())
                };

                var enrollmentActions = enrollmentList.OrderBy(x => Guid.NewGuid());
                foreach (var enrollmentAction in enrollmentActions)
                {
                    enrollmentAction();
                }

                ISemanticBroker broker = semanticBuilder.CreateBroker();

                // act
                await broker.On(new[] { new TestClassA() }).Output<TestClassB[]>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<TestClassB[]>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<List<TestClassB>>();
                await broker.On(new List<TestClassA> { new TestClassA() }).Output<IEnumerable<TestClassB>>();
                await broker.On(new[] { new TestClassA() }).Output<List<TestClassB>>();
            }
        }
    }
}