namespace xUnit.Paradigms.Tests
{
    public class AClassWithTwoIntArguments
    {
        private readonly int _i;
        private readonly int _i2;

        public int I
        {
            get { return _i; }
        }

        public int I2
        {
            get { return _i2; }
        }

        public AClassWithTwoIntArguments(int i, int i2)
        {
            _i = i;
            _i2 = i2;
        }

        public void TestMethod()
        {

        }
    }
}