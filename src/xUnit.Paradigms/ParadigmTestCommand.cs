using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xunit.Sdk;

namespace xUnit.Paradigms
{
    public class ParadigmTestCommand : TestCommand
    {
        private readonly ITestCommand _innerCommand;
        private readonly object _testClassInstance;

        public object TestClassInstance
        {
            get { return _testClassInstance; }
        }

        public ParadigmTestCommand(IMethodInfo testMethod, ITestCommand innerCommand, string displayName, object testClassInstance)
            : base(testMethod, displayName, MethodUtility.GetTimeoutParameter(testMethod))
        {
            _innerCommand = innerCommand;
            _testClassInstance = testClassInstance;
        }

        public override MethodResult Execute(object testClass)
        {
            try
            {
                _innerCommand.Execute(_testClassInstance);
                return new PassedResult(testMethod, DisplayName);
            }
            catch (Exception ex)
            {
                return new FailedResult(testMethod, ex, DisplayName);
            }

            //object testInstance = null;
            //
            //try
            //{
            //    testInstance = _paradigmExemplar.CreateTestInstance();
            //}
            //catch (TargetInvocationException ex)
            //{
            //    ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            //}
            //
            //try
            //{
            //    _innerCommand.Execute(testInstance);
            //    return new PassedResult(testMethod, DisplayName);
            //}
            //catch (Exception ex)
            //{
            //    return new FailedResult(testMethod, ex, DisplayName);
            //}
            //finally
            //{
            //    var disposable = testInstance as IDisposable;
            //    if (disposable != null) disposable.Dispose();
            //}
        }


        public override bool ShouldCreateInstance { get { return false; } }
    }
}