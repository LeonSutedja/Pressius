using System;
using System.Linq;
using System.Reflection;

namespace Pressius.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool IsNullableProperty(this PropertyInfo prop)
            => prop.PropertyType.IsGenericType &&
               prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static string GetNullablePropertyName(this PropertyInfo prop)
            => prop.PropertyType.GetGenericArguments().First().Name;
    }
}