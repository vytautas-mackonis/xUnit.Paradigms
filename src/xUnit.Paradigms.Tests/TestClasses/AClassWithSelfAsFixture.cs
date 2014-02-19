using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithSelfAsFixture : IUseFixture<AClassWithSelfAsFixture>
    {
        public void SetFixture(AClassWithSelfAsFixture data)
        {
            
        }

        public void TestMethod()
        {

        }
    }
}