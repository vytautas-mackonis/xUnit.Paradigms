using System;
using System.Reflection;
using System.Xml;
using Xunit.Sdk;

namespace xUnit.Paradigms
{
    public class ParadigmTestCommand : TestCommand
    {
        private readonly ITestCommand _innerCommand;
        private readonly Type _testClassType;
        private readonly ParameterInfo[] _paradigmParameters;
        private readonly object[] _paradigmData;

        public ParadigmTestCommand(IMethodInfo testMethod, ITestCommand innerCommand, Type testClassType, ParameterInfo[] paradigmParameters, object[] paradigmData)
            : base(testMethod, MethodUtility.GetDisplayName(testMethod), MethodUtility.GetTimeoutParameter(testMethod))
        {
            _innerCommand = innerCommand;
            _testClassType = testClassType;
            _paradigmParameters = paradigmParameters;
            _paradigmData = paradigmData;

            var parameters = new string[_paradigmData.Length];
            for (int i = 0; i < _paradigmData.Length; i++)
            {
                parameters[i] = _paradigmParameters[i].Name + ": " + ((_paradigmData[i] is string) ? "\"" + _paradigmData[i] + "\"" : _paradigmData[i].ToString());
            }

            DisplayName = _innerCommand.DisplayName.Replace(_testClassType.Name, string.Format("{0}({1})", _testClassType.Name, string.Join(", ", parameters)));
        }

        public override MethodResult Execute(object testClass)
        {
            object testInstance = null;

            try
            {
                testInstance = CreateTestInstance();
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            try
            {
                _innerCommand.Execute(testInstance);
                return new PassedResult(testMethod, DisplayName);
            }
            catch (Exception ex)
            {
                return new FailedResult(testMethod, ex, DisplayName);
            }
            finally
            {
                var disposable = testInstance as IDisposable;
                if (disposable != null) disposable.Dispose();
            }
        }


        public override bool ShouldCreateInstance { get { return false; } }

        private object CreateTestInstance()
        {
            return Activator.CreateInstance(_testClassType, _paradigmData);
        }
    }
}