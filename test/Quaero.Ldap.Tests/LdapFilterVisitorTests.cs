using Quaero.Tests;

namespace Quaero.Ldap.Tests;

public class LdapFilterVisitorTests : StringFilterVisitorTestBase
{
    [Fact]
    public Task Absencefilter() => Verify(ToString(Equal("diabledDate", (object?)null)));

    [Fact]
    public Task PresenceFilter() => Verify(ToString(NotEqual("diabledDate", (object?)null)));

    protected override string ToString(Filter filter) => filter.ToLdapQuery();
}
