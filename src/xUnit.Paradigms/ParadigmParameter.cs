using System.Reflection;

namespace xUnit.Paradigms
{
    public class ParadigmParameter
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly object _value;

        public ParadigmParameter(ParameterInfo parameterInfo, object value)
        {
            _parameterInfo = parameterInfo;
            _value = value;
        }

        public object Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _parameterInfo.Name + ": " + ((_value is string) ? "\"" + _value + "\"" : _value.ToString());
        }
    }
}