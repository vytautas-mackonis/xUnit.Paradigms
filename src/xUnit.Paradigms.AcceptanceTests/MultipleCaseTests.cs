using TestUtility;
using Xunit;
using Xunit.Extensions;

namespace xUnit.Paradigms.AcceptanceTests
{
    public class MultipleCaseTests : AcceptanceTestInNewAppDomain
    {
        private const string code = @"using Xunit;
using xUnit.Paradigms;

[Paradigm]
[ParadigmInlineData(""foo"", 1)]
[ParadigmInlineData(""bar"", 2)]
public class MultipleCaseTests
{
    private readonly string _fooField;
    private readonly int _oneField;

    public MultipleCaseTests(string field1, int field2)
    {
        _fooField = field1;
        _oneField = field2;
    }

    [Fact]
    public void TheTest()
    {
        Assert.Equal(""bar"", _fooField);
        Assert.Equal(2, _oneField);
    }
}";

        [Fact]
        public void PassingTestPasses()
        {
            var assemblyNode = ExecuteWithReferences(code, "xunit.paradigms.dll");

            var passingTestResult = ResultXmlUtility.GetResult(assemblyNode, 0, 1);
            passingTestResult.ShouldHaveName("MultipleCaseTests(field1: \"bar\", field2: 2).TheTest");
            passingTestResult.ShouldHavePassed();
        }

        [Fact]
        public void FailingTestFails()
        {
            var assemblyNode = ExecuteWithReferences(code, "xunit.extensions.dll", "xunit.paradigms.dll");

            var passingTestResult = ResultXmlUtility.GetResult(assemblyNode, 0, 0);
            passingTestResult.ShouldHaveName("MultipleCaseTests(field1: \"foo\", field2: 1).TheTest");
            passingTestResult.ShouldHaveFailed();
        }
    }
}