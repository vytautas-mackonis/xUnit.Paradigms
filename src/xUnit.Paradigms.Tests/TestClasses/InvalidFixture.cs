using System;
using System.Threading;

namespace xUnit.Paradigms.Tests
{
    public class InvalidFixture
    {
        public InvalidFixture()
        {
            throw new Exception("Invalid fixture");
        }
    }
}