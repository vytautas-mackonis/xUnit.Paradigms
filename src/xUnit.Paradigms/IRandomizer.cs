using System;
using System.Runtime.InteropServices;

namespace xUnit.Paradigms
{
    public interface IRandomizer
    {
        int Next(int maxValue);
    }

    public class DefaultRandomizer : IRandomizer
    {
        private readonly Random _random = new Random();

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }
    }
}