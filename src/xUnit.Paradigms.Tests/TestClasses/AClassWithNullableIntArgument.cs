using Xunit;

namespace xUnit.Paradigms.Tests
{
    class AClassWithNullableIntArgument
    {
        private readonly int? _s;

        public int? ArgumentValue
        {
            get { return _s; }
        }

        public AClassWithNullableIntArgument(int? s)
        {
            _s = s;
        }

        [Fact]
        public void TestMethod()
        {
            Assert.Equal(_s, 1);
        }
    }
}