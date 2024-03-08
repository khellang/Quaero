using System.Reflection;

namespace Quaero;

internal static class SetMethods<T>
{
    public static readonly MethodInfo Contains =
        typeof(ISet<T>).GetMethod(nameof(ISet<T>.Contains), new[] { typeof(T) })!;
}