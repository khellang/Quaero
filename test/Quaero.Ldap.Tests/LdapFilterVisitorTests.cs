using Quaero.Tests;

namespace Quaero.Ldap.Tests;

public class LdapFilterVisitorTests : StringFilterVisitorTestBase
{
    protected override string Convert(Filter filter) => filter.ToLdapQuery();
}
