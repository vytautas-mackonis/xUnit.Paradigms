using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using FrontIT.Matchers;
using Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk.Utilities;
using Assert = FrontIT.Matchers.Assert;

namespace xUnit.Paradigms.Tests
{
    public class ConstructorInvokingObjectFactoryTests
    {
        [Theory]
        [InlineData(typeof(AClassWithStringArgument), new object[] {"foo"})]
        [InlineData(typeof(AClassWithTwoIntArguments), new object[] { 3, 4 })]
        public void UsesCorrectType(Type requestedType, object[] arguments)
        {
            var sut = new ConstructorInvokingObjectFactory(requestedType, arguments);
            var actual = sut.CreateInstance();

            Assert.IsType(requestedType, actual);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public void PassesCorrectArguments(string argument)
        {
            var sut = new ConstructorInvokingObjectFactory(typeof (AClassWithStringArgument), new object[] {argument});

            var expected = new AClassWithStringArgument(argument);
            var actual = sut.CreateInstance();

            Assert.IsType<AClassWithStringArgument>(actual);
            Assert.That((AClassWithStringArgument)actual, Is.StructurallyEqualTo(expected));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public void UnwrapsTargetInvocationWhenCreating(string exceptionMessage)
        {
            var sut = new ConstructorInvokingObjectFactory(typeof (AClassThatThrowsExceptionWhenConstructed), new object[] {exceptionMessage});

            var thrown = Assert.Throws<Exception>(() => sut.CreateInstance());
            Assert.Equal(exceptionMessage, thrown.Message);
        }

        [Theory]
        [InlineData(typeof(AClassWithStringArgument))]
        [InlineData(typeof(AClassWithThreeConstructors))]
        public void SavesPassedInType(Type typeToPassIn)
        {
            var sut = new ConstructorInvokingObjectFactory(typeToPassIn, new object[0]);
            Assert.Equal(typeToPassIn, sut.TestClassType);
        }

        [Fact]
        public void SavesPassedInArguments()
        {
            var constructorArguments = new object[0];
            var sut = new ConstructorInvokingObjectFactory(typeof(AClassWithStringArgument), constructorArguments);
            Assert.Same(constructorArguments, sut.ConstructorArguments);
        }
    }
}
