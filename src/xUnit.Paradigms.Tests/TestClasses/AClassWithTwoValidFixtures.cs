using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithTwoValidFixtures : IUseFixture<ValidFixture1>, IUseFixture<ValidFixture2>
    {
        public void SetFixture(ValidFixture1 data)
        {
            
        }

        public void SetFixture(ValidFixture2 data)
        {
            
        }

        public void TestMethod()
        {

        }
    }
}