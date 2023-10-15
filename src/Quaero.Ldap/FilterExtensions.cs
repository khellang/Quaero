using Quaero.Ldap;

// ReSharper disable once CheckNamespace
namespace Quaero;

/// <summary>
/// LDAP-specific filter extensions.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Converts the specified <paramref name="filter"/> to an LDAP query string.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    /// <returns>An LDAP query string representing the specified <paramref name="filter"/>.</returns>
    public static string ToLdapQuery(this Filter filter) =>
        LdapFilterVisitor.Instance.Visit(filter);
}
