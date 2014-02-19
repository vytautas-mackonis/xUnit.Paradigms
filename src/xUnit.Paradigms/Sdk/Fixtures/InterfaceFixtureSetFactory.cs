using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Fixtures
{
    public class InterfaceFixtureSetFactory : IFixtureSetFactory
    {
        public IFixtureSet CreateFixturesFor(ITypeInfo type)
        {
            var fixtureDictionary = new Dictionary<MethodInfo, object>();

            foreach (var @interface in FindFixtureInterfaces(type.Type))
            {
                var dataType = @interface.GetGenericArguments()[0];
                if (dataType == type.Type) throw new InvalidOperationException("Cannot use a test class as its own fixture data");

                var fixtureData = CreateFixture(dataType);

                var method = @interface.GetMethod("SetFixture", new[] { dataType });
                fixtureDictionary[method] = fixtureData;
            }

            return new FixtureSet(fixtureDictionary);
        }

        private static object CreateFixture(Type dataType)
        {
            return new ConstructorInvokingObjectFactory(dataType, new object[0]).CreateInstance();
        }

        private IEnumerable<Type> FindFixtureInterfaces(Type classType)
        {
            return classType.GetInterfaces()
                .Where(@interface => @interface.IsGenericType 
                    && @interface.GetGenericTypeDefinition() == typeof (IUseFixture<>));
        }
    }
}