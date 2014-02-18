using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace xUnit.Paradigms
{
    public class ParadigmTestClassCommand : ITestClassCommand
    {
        private readonly Dictionary<MethodInfo, object> _fixtures = new Dictionary<MethodInfo, object>();

        public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
        {
            return 0;
        }

        public Exception ClassFinish()
        {
            foreach (object fixtureData in _fixtures.Values)
                try
                {
                    var disposable = fixtureData as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
                catch (Exception ex)
                {
                    return ex;
                }

            return null;
        }

        public Exception ClassStart()
        {
            try
            {
                foreach (var @interface in TypeUnderTest.Type.GetInterfaces())
                {
                    if (@interface.IsGenericType)
                    {
                        var genericDefinition = @interface.GetGenericTypeDefinition();

                        if (genericDefinition == typeof(IUseFixture<>))
                        {
                            var dataType = @interface.GetGenericArguments()[0];
                            if (dataType == TypeUnderTest.Type)
                                throw new InvalidOperationException("Cannot use a test class as its own fixture data");

                            object fixtureData = null;

                            try
                            {
                                fixtureData = Activator.CreateInstance(dataType);
                            }
                            catch (TargetInvocationException ex)
                            {
                                return ex.InnerException;
                            }

                            var method = @interface.GetMethod("SetFixture", new Type[] { dataType });
                            _fixtures[method] = fixtureData;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
        {
            var skipReason = MethodUtility.GetSkipReason(testMethod);

            if (skipReason != null)
            {
                yield return new SkipCommand(testMethod, MethodUtility.GetDisplayName(testMethod), skipReason);
            }
            else
            {
                var constructor = testMethod.Class.Type.GetConstructors().Single();
                var paradigmParameters = constructor.GetParameters();

                foreach (var constructorDataSet in GetData(constructor))
                {
                    foreach (var command in MethodUtility.GetTestCommands(testMethod))
                    {
                        yield return new ParadigmTestCommand(testMethod, new FixtureCommand(command, _fixtures), testMethod.Class.Type, paradigmParameters, constructorDataSet);
                    }
                }
            }
        }


        private static IEnumerable<object[]> GetData(ConstructorInfo method)
        {
            foreach (ParadigmDataAttribute attr in method.ReflectedType.GetCustomAttributes(typeof(ParadigmDataAttribute), false))
            {
                var parameterInfos = method.GetParameters();
                var parameterTypes = new Type[parameterInfos.Length];

                for (var idx = 0; idx < parameterInfos.Length; idx++)
                    parameterTypes[idx] = parameterInfos[idx].ParameterType;

                var attrData = attr.GetData(method, parameterTypes);
                if (attrData == null) continue;

                foreach (var dataItems in attrData)
                    yield return dataItems;
            }
        }

        public IEnumerable<IMethodInfo> EnumerateTestMethods()
        {
            return TypeUtility.GetTestMethods(TypeUnderTest);
        }

        public bool IsTestMethod(IMethodInfo testMethod)
        {
            return MethodUtility.IsTest(testMethod);
        }

        public object ObjectUnderTest { get; private set; }
        public ITypeInfo TypeUnderTest { get; set; }
    }
}