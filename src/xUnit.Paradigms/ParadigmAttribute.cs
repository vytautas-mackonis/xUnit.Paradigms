using System.Text;
using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk;

namespace xUnit.Paradigms
{
    public class ParadigmAttribute : RunWithAttribute
    {
        public ParadigmAttribute()
            : base(typeof(ParadigmTestClassCommand))
        {
        }
    }
}
