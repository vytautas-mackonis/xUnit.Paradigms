xUnit.Paradigms
===============

Parametrized test class per fixture support for xUnit.NET.

Motivation
===
xUnit.Extensions assembly allows to define data theories that are parametrized tests. These tests eliminate some duplication when the test code only differs by concrete parameters. However, in some cases it still leaves some duplicate code. Consider these tests for ASP .NET Web API controller:
```csharp
[Theory]
[InlineData(2, 2, 1)]
[InlineData(6, 2, 3)]
public void DivisionControllerPerformsDivision(int input, int operand, int expected)
{
    var sut = new DivisionController();
    var response = sut.Get(input, operand);
    var actual = response.GetContent<int>();
    Assert.Equal(expected, actual);

}

[Theory]
[InlineData(2, 2)]
[InlineData(6, 2)]
public void DivisionControllerReturnsHttpOk(int input, int operand)
{
    var sut = new DivisionController();
    var response = sut.Get(input, operand);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

}
```

As you can see, the setup and act phases are the same, while the assert phase is different. We could merge these two tests into one thus eliminating duplication, but then we would mix two unrelated assertions into the same test, which is a test smell. We could refactor similar code into the class constructor, but then we would lose ability to parametrize the test - the Get method behaves differently with different parameters.

xUnit.Paradigms to the rescue
===

Wtih xUnit.Paradigms we can write these tests in the following way:
```csharp
[Paradigm]
[ParadigmInlineData(2, 2, 1)]
[ParadigmInlineData(6, 2, 3)]
public class DivisionControllerSuccessTests
{
    private readonly int _actual;
    private readonly int _expected;
    private readonly HttpStatusCode _statusCode;
    
    public DivisionControllerSuccessTests(int input, int operand, int expected)
    {
        var sut = new DivisionController();
        var response = sut.Get(_input, _operand);
        _expected = expected;
        _actual = response.GetContent<int>();
        _statusCode = response.StatusCode;
    }
    
    [Fact]
    public void DivisionControllerPerformsDivision()
    {
        Assert.Equal(_expected, _actual);
    }
    
    [Fact]
    public void DivisionControllerReturnsHttpOk()
    {
        Assert.Equal(HttpStatusCode.OK, _statusCode);
    }
}
```
As you can see, instead of parametrizing methods we parametrized class constructor. Then we can move the common code into the constructor and the tests themselves will only do the assertions. The concept is the same as data theories, just different attribute names. And you can also mix `[Paradigm]` attribute with a `[Theory]`.
