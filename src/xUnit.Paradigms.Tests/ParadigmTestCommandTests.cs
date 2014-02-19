using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrontIT.Matchers;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using xUnit.Paradigms.Sdk;
using xUnit.Paradigms.Sdk.Utilities;
using Xunit.Sdk;

namespace xUnit.Paradigms.Tests
{
    public class ParadigmTestCommandTests
    {
        [Theory]
        [DefaultAutoData]
        public void DisplayNameIsSaved([Frozen] string displayName, ParadigmTestCommand sut)
        {
            Assert.Equal(displayName, sut.DisplayName);
        }

        [Theory]
        [DefaultAutoData]
        public void InstanceFactoryIsSaved([Frozen] IObjectFactory instanceFactory, ParadigmTestCommand sut)
        {
            Assert.Same(instanceFactory, sut.InstanceFactory);
        }

        [Theory]
        [DefaultAutoData]
        public void ShouldCreateInstanceIsFalse(ParadigmTestCommand sut)
        {
            Assert.False(sut.ShouldCreateInstance);
        }

        [Theory]
        [DefaultAutoData]
        public void InnerCommandIsExecutedWithCreatedInstance([Frozen] IObjectFactory instanceFactory, [Frozen] ITestCommand innerCommand, ParadigmTestCommand sut, object createdTestClass)
        {
            Mock.Get(instanceFactory).Setup(x => x.CreateInstance()).Returns(createdTestClass);

            sut.Execute(null);

            Mock.Get(innerCommand).Verify(x => x.Execute(createdTestClass));
        }

        [Theory]
        [DefaultAutoData]
        public void PassedResultIsReturnedIfInnerCommandPasses([Frozen] string displayName, [Frozen] ITestCommand innerCommand, ParadigmTestCommand sut, PassedResult innerCommandResult)
        {
            Mock.Get(innerCommand).Setup(x => x.Execute(It.IsAny<object>()))
                .Returns(innerCommandResult);

            var result = sut.Execute(null);

            Assert.That(result, Describe.Object<MethodResult>()
                .Cast<PassedResult>(c => c.Property(x => x.DisplayName, Is.EqualTo(displayName))));
        }

        [Theory]
        [DefaultAutoData]
        public void FailedResultIsReturnedIfInnerCommandFails([Frozen] string displayName, [Frozen] ITestCommand innerCommand, ParadigmTestCommand sut, FailedResult innerCommandResult)
        {
            Mock.Get(innerCommand).Setup(x => x.Execute(It.IsAny<object>()))
                .Returns(innerCommandResult);

            var result = sut.Execute(null);

            Assert.That(result, Describe.Object<MethodResult>()
                .Cast<FailedResult>(c => c.Property(x => x.DisplayName, Is.EqualTo(displayName))
                    .Property(x => x.ExceptionType, Is.EqualTo(innerCommandResult.ExceptionType))
                    .Property(x => x.MethodName, Is.EqualTo(innerCommandResult.MethodName))
                    .Property(x => x.TypeName, Is.EqualTo(innerCommandResult.TypeName))
                    .Property(x => x.Traits, Is.EqualTo(innerCommandResult.Traits))
                    .Property(x => x.Message, Is.EqualTo(innerCommandResult.Message))
                    .Property(x => x.StackTrace, Is.EqualTo(innerCommandResult.StackTrace))));
        }

        [Theory]
        [DefaultAutoData]
        public void FailedResultIsReturnedIfInstanceCreationThrows([Frozen] string displayName, [Frozen] IObjectFactory instanceFactory, [Frozen] IMethodInfo methodInfo, ParadigmTestCommand sut, Exception ex)
        {
            Mock.Get(instanceFactory).Setup(x => x.CreateInstance())
                .Throws(ex);

            var result = sut.Execute(null);

            MethodResult expected = new FailedResult(methodInfo, ex, displayName);

            Assert.That(result, Is.StructurallyEqualTo(expected));
        }

        [Theory]
        [DefaultAutoData]
        public void FailedResultIsReturnedIfInnerCommandThrows([Frozen] string displayName, [Frozen] ITestCommand innerCommand, [Frozen] IMethodInfo methodInfo, ParadigmTestCommand sut, Exception ex)
        {
            Mock.Get(innerCommand).Setup(x => x.Execute(It.IsAny<object>()))
                .Throws(ex);

            var result = sut.Execute(null);

            MethodResult expected = new FailedResult(methodInfo, ex, displayName);

            Assert.That(result, Is.StructurallyEqualTo(expected));
        }

        [Theory]
        [DefaultAutoData]
        public void TestInstanceIsDisposedAfter([Frozen] IObjectFactory instanceFactory, [Frozen] ITestCommand innerCommand, ParadigmTestCommand sut, IDisposable createdTestClass, PassedResult commandResult)
        {
            Mock.Get(instanceFactory).Setup(x => x.CreateInstance()).Returns(createdTestClass);

            Mock.Get(innerCommand).Setup(x => x.Execute(It.IsAny<object>()))
                .Returns(commandResult);

            sut.Execute(null);

            Mock.Get(createdTestClass).Verify(x => x.Dispose());
        }

        [Theory]
        [DefaultAutoData]
        public void TestInstanceIsDisposedAfterIfCommandFails([Frozen] IObjectFactory instanceFactory, [Frozen] ITestCommand innerCommand, ParadigmTestCommand sut, IDisposable createdTestClass)
        {
            Mock.Get(instanceFactory).Setup(x => x.CreateInstance()).Returns(createdTestClass);

            Mock.Get(innerCommand).Setup(x => x.Execute(It.IsAny<object>()))
                .Throws<Exception>();

            sut.Execute(null);

            Mock.Get(createdTestClass).Verify(x => x.Dispose());
        }
    }
}
