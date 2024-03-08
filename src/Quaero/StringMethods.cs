using System.Reflection;

namespace Quaero;

internal static class StringMethods
{
    public static readonly MethodInfo StartsWith = GetMethod(nameof(string.StartsWith));

    public static readonly MethodInfo EndsWith = GetMethod(nameof(string.EndsWith));

    public static readonly MethodInfo Contains = GetMethod(nameof(string.Contains));

    private static MethodInfo GetMethod(string methodName) =>
        typeof(string).GetMethod(methodName, new[] { typeof(string) })!;
}