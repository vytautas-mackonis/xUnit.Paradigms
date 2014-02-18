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
        private IParadigmExemplar[] _paradigmExemplars;
        private readonly Dictionary<MethodInfo, object> _fixtures = new Dictionary<MethodInfo, object>();
        private IExemplarFactory _exemplarFactory = new AttributeExemplarFactory();
        private IRandomizer _randomizer = new DefaultRandomizer();

        public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
        {
            return Randomizer.Next(testsLeftToRun.Count);
        }

        public Exception ClassFinish()
        {
            //foreach (object fixtureData in _fixtures.Values)
            //    try
            //    {
            //        var disposable = fixtureData as IDisposable;
            //        if (disposable != null)
            //            disposable.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        return ex;
            //    }

            return null;
        }

        public Exception ClassStart()
        {
            try
            {
                _paradigmExemplars = _exemplarFactory.CreateExemplarsFor(TypeUnderTest);

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }

            //try
            //{
            //    foreach (var @interface in TypeUnderTest.Type.GetInterfaces())
            //    {
            //        if (@interface.IsGenericType)
            //        {
            //            var genericDefinition = @interface.GetGenericTypeDefinition();
            //
            //            if (genericDefinition == typeof(IUseFixture<>))
            //            {
            //                var dataType = @interface.GetGenericArguments()[0];
            //                if (dataType == TypeUnderTest.Type)
            //                    throw new InvalidOperationException("Cannot use a test class as its own fixture data");
            //
            //                object fixtureData = null;
            //
            //                try
            //                {
            //                    fixtureData = Activator.CreateInstance(dataType);
            //                }
            //                catch (TargetInvocationException ex)
            //                {
            //                    return ex.InnerException;
            //                }
            //
            //                var method = @interface.GetMethod("SetFixture", new Type[] { dataType });
            //                _fixtures[method] = fixtureData;
            //            }
            //        }
            //    }
            //
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    return ex;
            //}
        }


        public IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
        {
            var skipReason = MethodUtility.GetSkipReason(testMethod);
            
            if (skipReason != null)
            {
                return new[] { new SkipCommand(testMethod, MethodUtility.GetDisplayName(testMethod), skipReason) };
            }
            
            return _paradigmExemplars
                .SelectMany(e => e.CreateTestCommands(testMethod, _fixtures));
        }



        public IEnumerable<IMethodInfo> EnumerateTestMethods()
        {
            return TypeUtility.GetTestMethods(TypeUnderTest);
        }

        public bool IsTestMethod(IMethodInfo testMethod)
        {
            return MethodUtility.IsTest(testMethod);
        }

        public IExemplarFactory ExemplarFactory
        {
            get { return _exemplarFactory; }
            set { _exemplarFactory = value; }
        }

        public IRandomizer Randomizer
        {
            get { return _randomizer; }
            set { _randomizer = value; }
        }

        public object ObjectUnderTest { get; private set; }
        public ITypeInfo TypeUnderTest { get; set; }
    }
}