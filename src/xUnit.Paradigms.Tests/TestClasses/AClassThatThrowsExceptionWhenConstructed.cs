using System;

namespace xUnit.Paradigms.Tests
{
    public class AClassThatThrowsExceptionWhenConstructed
    {
        public AClassThatThrowsExceptionWhenConstructed(string message)
        {
            throw new Exception(message);
        }
    }
}