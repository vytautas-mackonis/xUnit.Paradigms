using System;

namespace xUnit.Paradigms.Sdk.Utilities
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