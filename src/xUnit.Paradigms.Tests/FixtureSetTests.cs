using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrontIT.Matchers;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk.Fixtures;
using Xunit.Sdk;

namespace xUnit.Paradigms.Tests
{
    public class FixtureSetTests
    {
        [Theory]
        [DefaultAutoData]
        public void SavesFixtures([Frozen] Dictionary<MethodInfo, object> fixtures, FixtureSet sut)
        {
            Assert.Same(fixtures, sut.Fixtures);
        }

        [Theory]
        [DefaultAutoData]
        public void WrapsPassedInCommandIntoFixtureCommand([Frozen] Dictionary<MethodInfo, object> fixtures, FixtureSet sut, ITestCommand command)
        {
            var result = sut.ApplyFixturesToCommand(command);

            Assert.IsType<FixtureCommand>(result);
            var fixtureCommand = (FixtureCommand) result;
            Assert.Same(command, fixtureCommand.InnerCommand);

            //can't do anything else since fixtures property is not exposed
            var savedFixtures = typeof (FixtureCommand).GetField("fixtures", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixtureCommand);
            Assert.Same(fixtures, savedFixtures);
        }

        [Theory]
        [DefaultAutoData]
        public void DisposesAllFixtures([Frozen] Dictionary<MethodInfo, IDisposable> fixtures)
        {
            var sut = new FixtureSet(fixtures.ToDictionary(x => x.Key, x => (object) x.Value));

            sut.Dispose();

            foreach (var fixture in fixtures.Values)
            {
                Mock.Get(fixture).Verify(x => x.Dispose());
            }
        }
    }
}
