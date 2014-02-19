using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Fixtures
{
    public class FixtureSet : IFixtureSet
    {
        private readonly Dictionary<MethodInfo, object> _fixtureDictionary;

        public FixtureSet(Dictionary<MethodInfo, object> fixtureDictionary)
        {
            _fixtureDictionary = fixtureDictionary;
        }

        public void Dispose()
        {
            foreach (var fixture in Fixtures.Values.Select(OptionalDisposable.Create)) fixture.Dispose();
        }

        public ITestCommand ApplyFixturesToCommand(ITestCommand command)
        {
            return new FixtureCommand(command, _fixtureDictionary);
        }

        public Dictionary<MethodInfo, object> Fixtures
        {
            get { return _fixtureDictionary; }
        }
    }
}