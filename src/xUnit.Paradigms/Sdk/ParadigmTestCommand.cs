using System;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk
{
    public class ParadigmTestCommand : TestCommand
    {
        private readonly ITestCommand _innerCommand;
        private readonly IObjectFactory _instanceFactory;


        public ParadigmTestCommand(IMethodInfo testMethod, ITestCommand innerCommand, string displayName, IObjectFactory instanceFactory)
            : base(testMethod, displayName, MethodUtility.GetTimeoutParameter(testMethod))
        {
            _innerCommand = innerCommand;
            _instanceFactory = instanceFactory;
        }

        public override MethodResult Execute(object testClass)
        {
            try
            {
                return TryExecute();
            }
            catch (Exception ex)
            {
                return new FailedResult(testMethod, ex, DisplayName);
            }
        }

        private MethodResult TryExecute()
        {
            using (var testInstance = OptionalDisposable.Create(_instanceFactory.CreateInstance()))
            {
                var innerCommandResult = _innerCommand.Execute(testInstance.Value);

                var failure = innerCommandResult as FailedResult;
                if (failure != null) return new FailedResult(failure.MethodName, failure.TypeName, DisplayName, failure.Traits, failure.ExceptionType, failure.Message, failure.StackTrace);

                return new PassedResult(testMethod, DisplayName);
            }
        }

        public override bool ShouldCreateInstance { get { return false; } }

        public ITestCommand InnerCommand
        {
            get { return _innerCommand; }
        }

        public IObjectFactory InstanceFactory { get { return _instanceFactory; } }
    }
}