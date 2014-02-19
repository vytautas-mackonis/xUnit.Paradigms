using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithInvalidFixture : IUseFixture<InvalidFixture>
    {
        public void SetFixture(InvalidFixture data)
        {
            
        }

        public void TestMethod()
        {

        }
    }
}