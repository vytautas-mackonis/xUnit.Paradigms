using Xunit;

namespace xUnit.Paradigms.Tests
{
    class AClassWithStringArgument
    {
        private readonly string _s;

        public object ArgumentValue
        {
            get { return _s; }
        }

        public AClassWithStringArgument(string s)
        {
            _s = s;
        }

        [Fact]
        public void TestMethod()
        {
            Assert.Equal(_s, "foo");
        }
    }
}