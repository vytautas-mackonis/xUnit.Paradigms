using System.Reflection;

namespace xUnit.Paradigms.Sdk.Exemplars
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
            return string.Format("{0}: {1}", _parameterInfo.Name, FormatValue());
        }

        private string FormatValue()
        {
            if (_value == null) return "null";
            if (_value is string) return "\"" + _value + "\"";
            return _value.ToString();
        }
    }
}