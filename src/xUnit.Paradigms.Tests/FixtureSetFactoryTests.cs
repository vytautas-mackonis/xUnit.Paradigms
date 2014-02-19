using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrontIT.Matchers;
using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk.Fixtures;
using Xunit.Sdk;
using Assert = FrontIT.Matchers.Assert;

namespace xUnit.Paradigms.Tests
{
    public class FixtureSetFactoryTests
    {
        [Theory]
        [DefaultAutoData(typeof(AClassWithOneValidFixture), new [] { typeof(ValidFixture1) })]
        [DefaultAutoData(typeof(AClassWithOneValidFixtureAndOneOtherInterface), new[] { typeof(ValidFixture1) })]
        [DefaultAutoData(typeof(AClassWithTwoValidFixtures), new [] { typeof(ValidFixture1), typeof(ValidFixture2) })]
        public void CreatesCorrectFixtures(Type testClass, Type[] fixtureTypes, InterfaceFixtureSetFactory sut)
        {
            var result = sut.CreateFixturesFor(Reflector.Wrap(testClass));
            Assert.IsType<FixtureSet>(result);
            var fixtureSet = (FixtureSet) result;

            Assert.That(fixtureSet.Fixtures, Matches.AllOf(fixtureTypes
                .Select(type =>
                {
                    var ifc = testClass.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IUseFixture<>) && x.GetGenericArguments()[0] == type);
                    return Has.Entry(Is.EqualTo(ifc.GetMethod("SetFixture", new[] {type})), Is.InstanceOf(type));
                })));
        }

        [Theory]
        [DefaultAutoData]
        public void ThrowsWhenClassUsesItselfAsFixture(InterfaceFixtureSetFactory sut)
        {
            Assert.Throws<InvalidOperationException>(() => sut.CreateFixturesFor(Reflector.Wrap(typeof (AClassWithSelfAsFixture))));
        }

        [Theory]
        [DefaultAutoData]
        public void UnwrapsTargetInvocationIfThrown(InterfaceFixtureSetFactory sut)
        {
            var ex = Assert.Throws<Exception>(() => sut.CreateFixturesFor(Reflector.Wrap(typeof(AClassWithInvalidFixture))));
            Assert.Equal("Invalid fixture", ex.Message);
        }
    }
}
