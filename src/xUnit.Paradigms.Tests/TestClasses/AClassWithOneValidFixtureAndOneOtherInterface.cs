using Xunit;

namespace xUnit.Paradigms.Tests
{
    public class AClassWithOneValidFixtureAndOneOtherInterface : IUseFixture<ValidFixture1>, IOtherInterface<ValidFixture2>
    {
        public void SetFixture(ValidFixture1 data)
        {

        }

        public void TestMethod()
        {

        }

        public void SetFixture(ValidFixture2 data)
        {
            
        }
    }

    public interface IOtherInterface<T>
    {
        void SetFixture(T data);
    }
}