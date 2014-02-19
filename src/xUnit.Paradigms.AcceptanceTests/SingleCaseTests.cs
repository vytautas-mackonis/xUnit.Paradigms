using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestUtility;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace xUnit.Paradigms.AcceptanceTests
{
    public class SingleCaseTests : AcceptanceTestInNewAppDomain
    {
        [Fact]
        public void PassingTestPasses()
        {
            const string code = @"using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms;

[Paradigm]
[ParadigmInlineData(""foo"", 1)]
public class SingleCaseTests
{
    private readonly string _fooField;
    private readonly int _oneField;

    public SingleCaseTests(string fooField, int oneField)
    {
        _fooField = fooField;
        _oneField = oneField;
    }

    [Theory]
    [InlineData(12L, ""bar"")]
    public void PassingTheory(long twelveParam, string barParam)
    {
        Assert.Equal(""foo"", _fooField);
        Assert.Equal(1, _oneField);
        Assert.Equal(12L, twelveParam);
        Assert.Equal(""bar"", barParam);
    }
}";

            var assemblyNode = ExecuteWithReferences(code, "xunit.extensions.dll", "xunit.paradigms.dll");

            var passingTestResult = ResultXmlUtility.GetResult(assemblyNode, 0, 0);

            passingTestResult.ShouldHaveName("SingleCaseTests(fooField: \"foo\", oneField: 1).PassingTheory(twelveParam: 12, barParam: \"bar\")");
            passingTestResult.ShouldHavePassed();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void FailingTestFails(int testIndex)
        {
            const string code = @"using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms;

[Paradigm]
[ParadigmInlineData(""foo"", 1)]
public class SingleCaseTests
{{
    private readonly string _fooField;
    private readonly int _oneField;

    public SingleCaseTests(string fooField, int oneField)
    {{
        _fooField = fooField;
        _oneField = oneField;
    }}

    [Theory]
    [InlineData(12L, ""bar"")]
    public void FailingTheory{0}(long twelveParam, string barParam)
    {{
        Assert.Equal(""foo"", barParam);
    }}
}}";

            var assemblyNode = ExecuteWithReferences(string.Format(code, testIndex), "xunit.extensions.dll", "xunit.paradigms.dll");

            var passingTestResult = ResultXmlUtility.GetResult(assemblyNode, 0, 0);
            passingTestResult.ShouldHaveName("SingleCaseTests(fooField: \"foo\", oneField: 1).FailingTheory" + testIndex + "(twelveParam: 12, barParam: \"bar\")");
            passingTestResult.ShouldHaveFailed();
        }
    }
}
