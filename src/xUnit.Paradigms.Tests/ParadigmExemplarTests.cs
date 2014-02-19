using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk;
using xUnit.Paradigms.Sdk.Exemplars;
using xUnit.Paradigms.Sdk.Fixtures;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Tests
{
    public class ParadigmExemplarTests
    {
        [Theory]
        [DefaultAutoData(typeof(AClassWithStringArgument), new object[] { "foo" })]
        [DefaultAutoData(typeof(AClassWithStringArgument), new object[] { "bar" })]
        [DefaultAutoData(typeof(AClassWithTwoIntArguments), new object[] { 1, 17 })]
        public void TestCommandsAreCreatedAndWrappedCorrectlyForEachFactAttribute(
            Type testClassType,
            object[] constructorParameters,
            IMethodInfo methodInfo, 
            IFixtureSet fixtureSet,
            MockFactAttribute[] factAttributes, 
            ITestCommand[][] factAttributeTestCommandSets,
            ITestCommand[][] fixtureTestCommandSets)
        {
            var sut = new ParadigmExemplar(Reflector.Wrap(testClassType), testClassType.GetConstructors()[0].GetParameters(), constructorParameters);

            Mock.Get(methodInfo).Setup(x => x.GetCustomAttributes(typeof (FactAttribute)))
                .Returns(factAttributes.Select(Reflector.Wrap));

            for (var i = 0; i < factAttributes.Length; i++)
            {
                var index = i;
                factAttributes[index].TestCommands = factAttributeTestCommandSets[index];

                for (var j = 0; j < factAttributeTestCommandSets[index].Length; j++)
                {
                    var jIndex = j;
                    Mock.Get(factAttributeTestCommandSets[index][jIndex]).SetupGet(x => x.DisplayName).Returns("");
                    Mock.Get(fixtureSet).Setup(x => x.ApplyFixturesToCommand(factAttributeTestCommandSets[index][jIndex])).Returns(fixtureTestCommandSets[index][jIndex]);
                }
            }

            var result = sut.CreateTestCommands(methodInfo, fixtureSet);

            foreach (var testCommand in result)
            {
                Assert.IsType<ParadigmTestCommand>(testCommand);

                var paradigmCommand = (ParadigmTestCommand) testCommand;
                Assert.IsType<ConstructorInvokingObjectFactory>(paradigmCommand.InstanceFactory);

                var instanceFactory = (ConstructorInvokingObjectFactory) paradigmCommand.InstanceFactory;
                Assert.Equal(constructorParameters, instanceFactory.ConstructorArguments);
                Assert.Equal(testClassType, instanceFactory.TestClassType);
            }

            var expectedUnderlyingCommands = fixtureTestCommandSets.SelectMany(x => x);
            Assert.Equal(expectedUnderlyingCommands, result.Cast<ParadigmTestCommand>().Select(x => x.InnerCommand));
        }

        [Theory]
        [DefaultAutoData(typeof(AClassWithStringArgument), new object[] { "foo" }, "TestMethod", "AClassWithStringArgument(s: \"foo\").TestMethod")]
        [DefaultAutoData(typeof(AClassWithStringArgument), new object[] { "bar" }, "TestMethod(1, 2)", "AClassWithStringArgument(s: \"bar\").TestMethod(1, 2)")]
        [DefaultAutoData(typeof(AClassWithStringArgument), new object[] { null }, "TestMethod(1, 2)", "AClassWithStringArgument(s: null).TestMethod(1, 2)")]
        [DefaultAutoData(typeof(AClassWithTwoIntArguments), new object[] { 1, 17 }, "baz", "AClassWithTwoIntArguments(i: 1, i2: 17).baz")]
        public void TestCommandNamesAreDescriptive(
            Type testClassType,
            object[] constructorParameters,
            string commandNameSuffix,
            string expectedCommandName,
            IMethodInfo methodInfo,
            IFixtureSet fixtureSet,
            ITestCommand returnedCommand
            )
        {
            var sut = new ParadigmExemplar(Reflector.Wrap(testClassType), testClassType.GetConstructors()[0].GetParameters(), constructorParameters);

            Mock.Get(methodInfo).Setup(x => x.GetCustomAttributes(typeof (FactAttribute)))
                .Returns(new[]
                {
                    Reflector.Wrap(new MockFactAttribute
                    {
                        TestCommands = new[] {returnedCommand}
                    })
                });

            Mock.Get(fixtureSet).Setup(x => x.ApplyFixturesToCommand(It.IsAny<ITestCommand>()))
                .Returns<ITestCommand>(c => c);

            Mock.Get(returnedCommand).SetupGet(x => x.DisplayName).Returns(testClassType.Name + "." + commandNameSuffix);

            var command = sut.CreateTestCommands(methodInfo, fixtureSet).First();

            Assert.Equal(expectedCommandName, command.DisplayName);
        }
    }
}
