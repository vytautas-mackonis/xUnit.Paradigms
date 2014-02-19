using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrontIT.Matchers;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace xUnit.Paradigms.Tests
{
    public class ParadigmInlineDataAttributeTests
    {
        [Theory]
        [DefaultAutoData]
        public void ReturnsPassedInData([Frozen] object[] data, ParadigmInlineDataAttribute sut, ConstructorInfo constructor, Type[] types)
        {
            Assert.That(sut.GetData(constructor, types), Is.ListOf(new [] { Is.SameAs(data) }));
        }
    }
}
