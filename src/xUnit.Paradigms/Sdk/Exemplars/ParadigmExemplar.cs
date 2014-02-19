using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xUnit.Paradigms.Sdk.Fixtures;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Exemplars
{
    public interface IParadigmExemplar
    {
        IEnumerable<ITestCommand> CreateTestCommands(IMethodInfo testMethod, IFixtureSet fixtureSet);
    }

    public class ParadigmExemplar : IParadigmExemplar
    {
        private readonly ITypeInfo _testClassType;
        private readonly ParadigmParameter[] _parameters;

        public ParadigmExemplar(ITypeInfo testClassType, ParameterInfo[] parameters, object[] parameterValues)
        {
            _testClassType = testClassType;
            _parameters = CreateParameters(parameters, parameterValues);
        }

        private ParadigmParameter[] CreateParameters(ParameterInfo[] parameters, object[] parameterValues)
        {
            ValidateParameters(parameters, parameterValues);
            return parameters.Select((param, index) => new ParadigmParameter(param, parameterValues[index])).ToArray();
        }

        private void ValidateParameters(ParameterInfo[] parameters, object[] parameterValues)
        {
            if (parameters.Length == parameterValues.Length && ParameterTypesMatch(parameters, parameterValues)) return;

            var expectedShape = CreateShape(parameters.Select(x => x.ParameterType));
            var actualShape = CreateShape(parameterValues.Select((x, index) => x == null ? parameters[index].ParameterType : x.GetType()));

            throw new InvalidParadigmExemplarException(string.Format("Invalid paradigm exemplar: expected it to have a shape of {0}, but it had a shape of {1}", expectedShape, actualShape));
        }

        private string CreateShape(IEnumerable<Type> types)
        {
            return string.Format("({0})", string.Join(", ", types.Select(x => x.FullName)));
        }

        private bool ParameterTypesMatch(ParameterInfo[] parameters, object[] parameterValues)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var value = parameterValues[i];

                if (!TypeExtensions.IsValueCompatible(parameterType, value)) return false;

            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", _testClassType.Type.Name, string.Join(", ", _parameters.Select(x => x.ToString())));
        }

        private string GetNameFor(ITestCommand command)
        {
            return command.DisplayName.Replace(_testClassType.Type.Name, ToString());
        }

        public IEnumerable<ITestCommand> CreateTestCommands(IMethodInfo testMethod, IFixtureSet fixtureSet)
        {
            foreach (var command in MethodUtility.GetTestCommands(testMethod))
            {
                yield return new ParadigmTestCommand(testMethod, fixtureSet.ApplyFixturesToCommand(command), GetNameFor(command), new ConstructorInvokingObjectFactory(_testClassType.Type, _parameters.Select(x => x.Value).ToArray()));
            }
        }
    }
}