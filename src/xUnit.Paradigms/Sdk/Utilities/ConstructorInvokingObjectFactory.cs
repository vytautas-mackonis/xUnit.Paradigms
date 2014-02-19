using System;
using System.Reflection;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk.Utilities
{
    public class ConstructorInvokingObjectFactory : IObjectFactory
    {
        private readonly Type _testClassType;
        private readonly object[] _constructorArguments;

        public object[] ConstructorArguments
        {
            get { return _constructorArguments; }
        }

        public Type TestClassType
        {
            get { return _testClassType; }
        }

        public ConstructorInvokingObjectFactory(Type testClassType, object[] constructorArguments)
        {
            _testClassType = testClassType;
            _constructorArguments = constructorArguments;
        }

        public object CreateInstance()
        {
            try
            {
                return Activator.CreateInstance(_testClassType, _constructorArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
                throw new Exception("Should never be reached");
            }
        }
    }
}