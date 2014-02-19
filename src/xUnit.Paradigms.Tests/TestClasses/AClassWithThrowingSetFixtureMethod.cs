using System;
using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithThrowingSetFixtureMethod : IUseFixture<ValidFixture1>
    {
        public void SetFixture(ValidFixture1 data)
        {
            throw new Exception("fixture method throws");
        }

        public void TestMethod()
        {

        }
    }
}