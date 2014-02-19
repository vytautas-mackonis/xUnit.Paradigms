using System;
using System.Collections.Generic;
using System.Linq;
using xUnit.Paradigms.Sdk.Exemplars;
using xUnit.Paradigms.Sdk.Fixtures;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Sdk
{
    public class ParadigmTestClassCommand : ITestClassCommand
    {
        private IParadigmExemplar[] _paradigmExemplars;
        private IExemplarFactory _exemplarFactory = new AttributeExemplarFactory();
        private IFixtureSetFactory _fixtureSetFactory = new InterfaceFixtureSetFactory();
        private IRandomizer _randomizer = new DefaultRandomizer();

        public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
        {
            return Randomizer.Next(testsLeftToRun.Count);
        }

        public Exception ClassFinish()
        {
            try
            {
                FixtureSet.Dispose();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public Exception ClassStart()
        {
            try
            {
                FixtureSet = _fixtureSetFactory.CreateFixturesFor(TypeUnderTest);
                _paradigmExemplars = _exemplarFactory.CreateExemplarsFor(TypeUnderTest);

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
                return new[] { new SkipCommand(testMethod, MethodUtility.GetDisplayName(testMethod), skipReason) };
            }
            
            return _paradigmExemplars
                .SelectMany(e => e.CreateTestCommands(testMethod, FixtureSet));
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

        public IFixtureSetFactory FixtureSetFactory
        {
            get { return _fixtureSetFactory; }
            set { _fixtureSetFactory = value; }
        }

        public IRandomizer Randomizer
        {
            get { return _randomizer; }
            set { _randomizer = value; }
        }

        public object ObjectUnderTest { get; private set; }
        public ITypeInfo TypeUnderTest { get; set; }
        public IFixtureSet FixtureSet { get; private set; }
    }
}