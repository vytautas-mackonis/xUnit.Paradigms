using System;

namespace xUnit.Paradigms.Sdk.Utilities
{
    public static class TypeExtensions
    {
        public static bool IsValueCompatible(Type type, object value)
        {
            if (value == null) return !type.IsValueType || (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition());
            return type.IsInstanceOfType(value);
        }
    }
}