using System;

namespace xUnit.Paradigms
{
    public class InvalidParadigmExemplarException : Exception
    {
        public InvalidParadigmExemplarException(string message)
            : base(message)
        {
        }
    }
}