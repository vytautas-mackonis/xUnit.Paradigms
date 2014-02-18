using System;
using System.Xml;
using Xunit;

namespace xUnit.Paradigms.AcceptanceTests
{
    public static class XmlTestResultExtensions
    {
        private static XmlAttribute GetAttribute(XmlNode testNode, string attributeName)
        {
            var attrib = testNode.Attributes[attributeName];
            if (attrib == null) throw new ArgumentException("Could not find attribute named " + attributeName + " in XML:\r\n" + testNode.OuterXml);
            return attrib;
        }

        private static void ShouldHaveResult(XmlNode testNode, string expectedResult)
        {
            var attribute = GetAttribute(testNode, "result");

            var failureMessage = string.Format("Expected a test result to be {0} but it was {1}. Result node XML:\r\n\r\n{2}", expectedResult, attribute.Value, testNode.OuterXml);

            Assert.True(attribute.Value == expectedResult, failureMessage);
        }

        public static void ShouldHavePassed(this XmlNode testNode)
        {
            ShouldHaveResult(testNode, "Pass");
        }

        public static void ShouldHaveFailed(this XmlNode testNode)
        {
            ShouldHaveResult(testNode, "Fail");
        }

        public static void ShouldHaveName(this XmlNode testNode, string name)
        {
            var attribute = GetAttribute(testNode, "name");
            Assert.Equal(name, attribute.Value);
        }
    }
}