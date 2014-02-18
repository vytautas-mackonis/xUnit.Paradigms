using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestUtility;
using Xunit;

namespace xUnit.Paradigms.AcceptanceTests
{
    public class FailureTests : AcceptanceTestInNewAppDomain
    {

        [Fact]
        public void ClassWithTwoConstructorsFails()
        {
            const string code = @"using Xunit;
using xUnit.Paradigms;

[Paradigm]
[ParadigmInlineData(1)]
public class AClassWithTwoConstructors
{
    public AClassWithTwoConstructors()
    {

    }

    public AClassWithTwoConstructors(int i)
    {

    }

    [Fact]
    public void TestMethod()
    {

    }
}";
            var assemblyNode = ExecuteWithReferences(code, "xunit.extensions.dll", "xunit.paradigms.dll");
            assemblyNode.ShouldHaveClassFailure();
        }

        [Fact]
        public void ClassWithMismatchedParametersFails()
        {
            const string code = @"using Xunit;
using xUnit.Paradigms;

[Paradigm]
[ParadigmInlineData(""foo"")]
public class TestClass
{
    public TestClass(int i)
    {

    }

    [Fact]
    public void TestMethod()
    {

    }
}";
            var assemblyNode = ExecuteWithReferences(code, "xunit.extensions.dll", "xunit.paradigms.dll");
            assemblyNode.ShouldHaveClassFailure();
        }
    }
}