using System;
using System.Collections.Generic;
using System.Reflection;

namespace xUnit.Paradigms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ParadigmInlineDataAttribute : ParadigmDataAttribute
    {
        private readonly object[] _dataValues;

        public ParadigmInlineDataAttribute(params object[] dataValues)
        {
            _dataValues = dataValues;
        }

        public override IEnumerable<object[]> GetData(ConstructorInfo constructor, Type[] parameterTypes)
        {
            yield return _dataValues;
        }
    }
}