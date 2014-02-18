using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Extensions;

namespace xUnit.Paradigms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class ParadigmDataAttribute : Attribute
    {
        public abstract IEnumerable<object[]> GetData(ConstructorInfo constructor, Type[] parameterTypes);
    }
}
