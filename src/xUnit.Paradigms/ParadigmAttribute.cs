using System.Text;
using Xunit;
using Xunit.Extensions;

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
