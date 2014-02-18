using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrontIT.Matchers;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;
using Assert = FrontIT.Matchers.Assert;

namespace xUnit.Paradigms.Tests
{
    public class ParadigmTestClassCommandTests
    {
        [Theory]
        [InlineData(typeof(AClassWithTwoConstructors))]
        [InlineData(typeof(AClassWithThreeConstructors))]
        public void ClassStartThrowsWhenClassHasMoreThanOneConstructor(Type classType)
        {
            var sut = new ParadigmTestClassCommand
            {
                TypeUnderTest = Reflector.Wrap(classType)
            };

            var exception = sut.ClassStart();
            Assert.IsType<InvalidParadigmException>(exception);
            Assert.Equal("The class " + classType.FullName + " must have exactly one constructor", exception.Message);
        }

        [Theory]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithStringArgument), new object[0], "Invalid paradigm exemplar: expected it to have a shape of (System.String), but it had a shape of ()")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithStringArgument), new object[] { 1 }, "Invalid paradigm exemplar: expected it to have a shape of (System.String), but it had a shape of (System.Int32)")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoStringArguments), new object[] { "" }, "Invalid paradigm exemplar: expected it to have a shape of (System.String, System.String), but it had a shape of (System.String)")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoStringArguments), new object[] { "", "", "" }, "Invalid paradigm exemplar: expected it to have a shape of (System.String, System.String), but it had a shape of (System.String, System.String, System.String)")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoStringArguments), new object[] { "", 1, "" }, "Invalid paradigm exemplar: expected it to have a shape of (System.String, System.String), but it had a shape of (System.String, System.Int32, System.String)")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoStringArguments), new object[] { null, 1, "" }, "Invalid paradigm exemplar: expected it to have a shape of (System.String, System.String), but it had a shape of (System.String, System.Int32, System.String)")]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoIntArguments), new object[] { 1, "" }, "Invalid paradigm exemplar: expected it to have a shape of (System.Int32, System.Int32), but it had a shape of (System.Int32, System.String)")]
        public void ClassStartThrowsUponMismatchingConstructors(
            Type classType, 
            object[] exemplarArguments, 
            string expectedMessage,
            ParadigmTestClassCommand sut, 
            IAttributeInfo attributeInfo, 
            ParadigmDataAttribute attribute)
        {
            Mock.Get(sut.TypeUnderTest).SetupGet(x => x.Type)
                .Returns(classType);

            Mock.Get(sut.TypeUnderTest).Setup(x => x.GetCustomAttributes(typeof (ParadigmDataAttribute)))
                .Returns(new[] { attributeInfo });

            Mock.Get(attributeInfo).Setup(x => x.GetInstance<ParadigmDataAttribute>())
                .Returns(attribute);

            Mock.Get(attribute).Setup(x => x.GetData(It.IsAny<ConstructorInfo>(), It.IsAny<Type[]>()))
                .Returns(new [] { exemplarArguments });

            var exception = sut.ClassStart();
            Assert.IsType<InvalidParadigmExemplarException>(exception);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Theory]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithStringArgument), new object[] { "" })]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoStringArguments), new object[] { "", null })]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoIntArguments), new object[] { 1, 1 })]
        [DontMockExemplarFactoryDataAttribute(typeof(AClassWithTwoObjectArguments), new object[] { "", new object[0] })]
        public void ClassStartDoesNotThrowUponMatchingConstructor(
            Type classType,
            object[] exemplarArguments,
            ParadigmTestClassCommand sut,
            IAttributeInfo attributeInfo,
            ParadigmDataAttribute attribute)
        {
            Mock.Get(sut.TypeUnderTest).SetupGet(x => x.Type)
                .Returns(classType);
        
            Mock.Get(sut.TypeUnderTest).Setup(x => x.GetCustomAttributes(typeof(ParadigmDataAttribute)))
                .Returns(new[] { attributeInfo });
        
            Mock.Get(attributeInfo).Setup(x => x.GetInstance<ParadigmDataAttribute>())
                .Returns(attribute);
        
            Mock.Get(attribute).Setup(x => x.GetData(It.IsAny<ConstructorInfo>(), It.IsAny<Type[]>()))
                .Returns(new[] { exemplarArguments });
        
            Assert.Null(sut.ClassStart());
        }

        [Theory]
        [DefaultAutoData]
        public void ExemplarFactoryIsUsedToCreateExemplars(
            [Frozen] IExemplarFactory exemplarFactory,
            ParadigmTestClassCommand sut,
            IParadigmExemplar[] exemplars,
            IEnumerable<ITestCommand>[] createdCommands,
            IMethodInfo methodToTest)
        {
            Mock.Get(exemplarFactory).Setup(x => x.CreateExemplarsFor(sut.TypeUnderTest))
                .Returns(exemplars);

            for (var i = 0; i < exemplars.Length; i++)
            {
                Mock.Get(exemplars[i]).Setup(x => x.CreateTestCommands(methodToTest, It.IsAny<Dictionary<MethodInfo, object>>()))
                    .Returns(createdCommands[i]);
            }


            sut.ClassStart();
            var commands = sut.EnumerateTestCommands(methodToTest);

            Assert.Equal(createdCommands.SelectMany(x => x), commands);
        }

        [Theory]
        [DefaultAutoData("foo")]
        [DefaultAutoData("bar")]
        public void SkippedMethodsAreHandled(
            string skipReason,
            ParadigmTestClassCommand sut,
            IMethodInfo methodToTest)
        {
            Mock.Get(methodToTest).Setup(x => x.GetCustomAttributes(typeof (FactAttribute)))
                .Returns(new[]
                {
                    Reflector.Wrap(new FactAttribute
                    {
                        Skip = skipReason
                    })
                });

            sut.ClassStart();
            var commands = sut.EnumerateTestCommands(methodToTest).ToList();
            Assert.That(commands, Is.OfLength(1));
            Assert.That(commands[0], Describe.Object<ITestCommand>()
                .Cast<SkipCommand>(c => c.Property(x => x.Reason, Is.EqualTo(skipReason))));
        }

        [Theory]
        [DefaultAutoData]
        public void AbstractMethodsAreSkipped(ParadigmTestClassCommand sut, IMethodInfo methodToTest)
        {
            Mock.Get(methodToTest).SetupGet(x => x.IsAbstract).Returns(true);
            Assert.False(sut.IsTestMethod(methodToTest));
        }

        [Theory]
        [DefaultAutoData]
        public void MethodsWithoutFactAttributeAreSkipped(ParadigmTestClassCommand sut, IMethodInfo methodToTest)
        {
            Mock.Get(methodToTest).Setup(x => x.HasAttribute(typeof (FactAttribute))).Returns(false);
            Assert.False(sut.IsTestMethod(methodToTest));
        }

        [Theory]
        [DefaultAutoData]
        public void MethodsWithFactAttributeAreConsideredTests(ParadigmTestClassCommand sut, IMethodInfo methodToTest)
        {
            Mock.Get(methodToTest).SetupGet(x => x.IsAbstract).Returns(false);
            Mock.Get(methodToTest).Setup(x => x.HasAttribute(typeof(FactAttribute))).Returns(true);
            Assert.True(sut.IsTestMethod(methodToTest));
        }

        [Theory]
        [DefaultAutoData(true, false, true)]
        [DefaultAutoData(false, true, false)]
        public void OnlyNonAbstractTypeMethodsAreConsideredTests(
            bool firstAbstract, 
            bool secondAbstract, 
            bool thirdAbstract, 
            ParadigmTestClassCommand sut, 
            IMethodInfo[] methods)
        {
            Mock.Get(sut.TypeUnderTest).Setup(x => x.GetMethods()).Returns(methods);

            foreach (var methodInfo in methods)
            {
                Mock.Get(methodInfo).Setup(x => x.HasAttribute(typeof (FactAttribute))).Returns(true);
            }

            Mock.Get(methods[0]).SetupGet(x => x.IsAbstract).Returns(firstAbstract);
            Mock.Get(methods[1]).SetupGet(x => x.IsAbstract).Returns(secondAbstract);
            Mock.Get(methods[2]).SetupGet(x => x.IsAbstract).Returns(thirdAbstract);

            var expected = new List<IMethodInfo>();
            if (!firstAbstract) expected.Add(methods[0]);
            if (!secondAbstract) expected.Add(methods[1]);
            if (!thirdAbstract) expected.Add(methods[2]);

            var actual = sut.EnumerateTestMethods();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [DefaultAutoData(true, false, true)]
        [DefaultAutoData(false, true, false)]
        public void OnlyTypeMethodsWithFactAttributeAreConsideredTests(
            bool firstHasFact,
            bool secondHasFact,
            bool thirdHasFact,
            ParadigmTestClassCommand sut,
            IMethodInfo[] methods)
        {
            Mock.Get(sut.TypeUnderTest).Setup(x => x.GetMethods()).Returns(methods);

            foreach (var methodInfo in methods)
            {
                Mock.Get(methodInfo).SetupGet(x => x.IsAbstract).Returns(false);
            }

            Mock.Get(methods[0]).Setup(x => x.HasAttribute(typeof(FactAttribute))).Returns(firstHasFact);
            Mock.Get(methods[1]).Setup(x => x.HasAttribute(typeof(FactAttribute))).Returns(secondHasFact);
            Mock.Get(methods[2]).Setup(x => x.HasAttribute(typeof(FactAttribute))).Returns(thirdHasFact);

            var expected = new List<IMethodInfo>();
            if (firstHasFact) expected.Add(methods[0]);
            if (secondHasFact) expected.Add(methods[1]);
            if (thirdHasFact) expected.Add(methods[2]);

            var actual = sut.EnumerateTestMethods();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [DefaultAutoData(1, 4)]
        [DefaultAutoData(1, 3)]
        [DefaultAutoData(5, 3)]
        public void RandomOrderIsUsedToChooseNextTest(int count, int randomValue, [Frozen] IRandomizer randomizer, ParadigmTestClassCommand sut, IFixture fixture)
        {
            Mock.Get(randomizer).Setup(x => x.Next(count)).Returns(randomValue);

            var methods = fixture.CreateMany<IMethodInfo>(count).ToList();

            var actual = sut.ChooseNextTest(methods);
            Assert.Equal(randomValue, actual);
        }
    }
}
