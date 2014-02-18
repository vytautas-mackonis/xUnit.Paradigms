using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;

namespace xUnit.Paradigms.Tests
{
    public class DefaultAutoDataAttribute : InlineAutoDataAttribute
    {
        public DefaultAutoDataAttribute()
            : this(new object[0])
        {
            
        }

        public DefaultAutoDataAttribute(params object[] values) : base(values)
        {
            AutoDataAttribute.Fixture.Customize(new AutoMoqCustomization());
            AutoDataAttribute.Fixture.Inject(AutoDataAttribute.Fixture);
        }
    }

    public class DontMockExemplarFactoryDataAttribute : DefaultAutoDataAttribute
    {
        public DontMockExemplarFactoryDataAttribute()
            : this(new object[0])
        {

        }

        public DontMockExemplarFactoryDataAttribute(params object[] values)
            : base(values)
        {
            AutoDataAttribute.Fixture.Inject<IExemplarFactory>(new AttributeExemplarFactory());
        }
    }
}
