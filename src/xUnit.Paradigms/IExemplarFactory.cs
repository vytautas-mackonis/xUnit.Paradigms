using Xunit.Sdk;

namespace xUnit.Paradigms
{
    public interface IExemplarFactory
    {
        IParadigmExemplar[] CreateExemplarsFor(ITypeInfo type);
    }
}