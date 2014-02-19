using System;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Fixtures
{
    public interface IFixtureSet : IDisposable
    {
        ITestCommand ApplyFixturesToCommand(ITestCommand command);
    }
}