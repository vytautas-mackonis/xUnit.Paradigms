namespace xUnit.Paradigms.Tests
{
    public class AClassWithTwoObjectArguments
    {
        private readonly object _o1;
        private readonly object _o2;

        public object O1
        {
            get { return _o1; }
        }

        public object O2
        {
            get { return _o2; }
        }

        public AClassWithTwoObjectArguments(object o1, object o2)
        {
            _o1 = o1;
            _o2 = o2;
        }

        public void TestMethod()
        {

        }
    }
}