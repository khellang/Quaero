using Quaero.Scim;

// ReSharper disable once CheckNamespace
namespace Quaero;

/// <summary>
/// SCIM-specific filter extensions.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Converts the specified <paramref name="filter"/> to a SCIM query string.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    /// <returns>A SCIM query string representing the specified <paramref name="filter"/>.</returns>
    public static string ToScimFilter(this Filter filter) =>
        ScimFilterVisitor.Instance.Visit(filter);
}
