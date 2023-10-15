using Quaero.MicrosoftGraph;

// ReSharper disable once CheckNamespace
namespace Quaero;

/// <summary>
/// Microsoft Graph-specific filter extensions.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Converts the specified <paramref name="filter"/> to a Microsoft Graph query string.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    /// <returns>A Microsoft Graph query string representing the specified <paramref name="filter"/>.</returns>
    public static string ToMicrosoftGraphQuery(this Filter filter) =>
        MicrosoftGraphFilterVisitor.Instance.Visit(filter);
}
