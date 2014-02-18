using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace xUnit.Paradigms
{
    public class AttributeExemplarFactory : IExemplarFactory
    {
        public IParadigmExemplar[] CreateExemplarsFor(ITypeInfo type)
        {
            var constructor = FindConstructor(type);

            return GetDataAttributes(type)
                .SelectMany(x => GetExemplars(type, constructor, x)).ToArray();
        }


        private ConstructorInfo FindConstructor(ITypeInfo typeUnderTest)
        {
            var constructors = typeUnderTest.Type.GetConstructors().ToArray();
            if (constructors.Length != 1) throw new InvalidParadigmException(string.Format("The class {0} must have exactly one constructor", typeUnderTest.Type.FullName));
            return constructors[0];
        }

        private static IEnumerable<ParadigmDataAttribute> GetDataAttributes(ITypeInfo type)
        {
            return type.GetCustomAttributes(typeof(ParadigmDataAttribute))
                .Select(x => x.GetInstance<ParadigmDataAttribute>());
        }


        private IEnumerable<IParadigmExemplar> GetExemplars(ITypeInfo typeUnderTest, ConstructorInfo constructor, ParadigmDataAttribute attribute)
        {

            //var parameterInfos = constructor.GetParameters();
            //var parameterTypes = new Type[parameterInfos.Length];
            //
            //for (var idx = 0; idx < parameterInfos.Length; idx++)
            //    parameterTypes[idx] = parameterInfos[idx].ParameterType;

            var attrData = attribute.GetData(null, null);
            //if (attrData == null) yield break;

            foreach (var dataItems in attrData)
            {
                yield return new ParadigmExemplar(typeUnderTest.Type, constructor.GetParameters(), dataItems);
            }
        }
    }
}