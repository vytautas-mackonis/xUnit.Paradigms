using Xunit;
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
}