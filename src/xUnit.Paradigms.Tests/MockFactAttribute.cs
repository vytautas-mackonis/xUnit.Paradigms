using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace xUnit.Paradigms.Tests
{
    public class MockFactAttribute : FactAttribute
    {
        public IEnumerable<ITestCommand> TestCommands = Enumerable.Empty<ITestCommand>();

        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            return TestCommands;
        }
    }
}