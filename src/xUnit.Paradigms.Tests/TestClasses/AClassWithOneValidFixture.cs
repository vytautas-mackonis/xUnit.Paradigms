using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithOneValidFixture : IUseFixture<ValidFixture1>
    {
        public void SetFixture(ValidFixture1 data)
        {
            
        }

        public void TestMethod()
        {

        }
    }
}