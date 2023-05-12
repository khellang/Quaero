
using Quaero.Ldap;

// ReSharper disable once CheckNamespace
namespace Quaero;

public static class FilterExtensions
{
    public static string ToLdapQuery(this Filter filter) => 
        LdapFilterVisitor.Transform(filter);
}
