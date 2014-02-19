using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Exemplars
{
    public interface IExemplarFactory
    {
        IParadigmExemplar[] CreateExemplarsFor(ITypeInfo type);
    }
}