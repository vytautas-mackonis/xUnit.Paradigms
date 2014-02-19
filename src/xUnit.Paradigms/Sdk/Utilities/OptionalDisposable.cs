using System;

namespace xUnit.Paradigms.Sdk.Utilities
{
    public class OptionalDisposable<T> : IDisposable
    {
        private readonly T _value;

        public OptionalDisposable(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }

        public void Dispose()
        {
            var disposable = _value as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }

    public static class OptionalDisposable
    {
        public static OptionalDisposable<T> Create<T>(T value)
        {
            return new OptionalDisposable<T>(value);
        }
    }
}