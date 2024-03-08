using System.Reflection;

namespace Quaero;

internal static class PropertyCache<T>
{
    // ReSharper disable once StaticMemberInGenericType
    public static readonly Dictionary<string, PropertyInfo> Properties = GetProperties();

    private static Dictionary<string, PropertyInfo> GetProperties() =>
        typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
}
