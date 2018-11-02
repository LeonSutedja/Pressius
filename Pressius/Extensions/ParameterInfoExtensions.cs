using System;
using System.Linq;
using System.Reflection;

namespace Pressius.Extensions
{
    internal static class ParameterInfoExtensions
    {
        public static bool IsNullableParameter(this ParameterInfo param)
            => param.ParameterType.IsGenericType &&
               param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static string GetNullableParameterName(this ParameterInfo param)
            => param.ParameterType.GetGenericArguments().First().Name;
    }
}