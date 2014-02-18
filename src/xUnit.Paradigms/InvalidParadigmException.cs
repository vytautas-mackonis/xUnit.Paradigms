using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xUnit.Paradigms
{
    public class InvalidParadigmException : Exception
    {
        public InvalidParadigmException(string message) : base(message)
        {
        }
    }
}
