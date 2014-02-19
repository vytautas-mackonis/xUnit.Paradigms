using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Fixtures
{
    public interface IFixtureSetFactory
    {
        IFixtureSet CreateFixturesFor(ITypeInfo type);
    }
}