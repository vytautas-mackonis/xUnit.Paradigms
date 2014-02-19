using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithDisposableFixture : IUseFixture<DisposableFixture>
    {
        public void SetFixture(DisposableFixture data)
        {
            
        }

        public void TestMethod()
        {

        }
    }
}