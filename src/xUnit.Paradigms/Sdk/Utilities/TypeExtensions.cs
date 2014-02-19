using System;

namespace xUnit.Paradigms.Sdk.Utilities
{
    public static class TypeExtensions
    {
        public static bool IsValueCompatible(Type type, object value)
        {
            if (value == null) return !type.IsValueType;
            return type.IsInstanceOfType(value);
        }
    }
}