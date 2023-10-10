using Quaero.Tests;

namespace Quaero.Ldap.Tests;

public class LdapFilterVisitorTests : StringFilterVisitorTestBase
{
    [Fact]
    public Task Absencefilter() => Assert(Equal("disabledDate", (object?)null));

    [Fact]
    public Task PresenceFilter() => Assert(NotEqual("disabledDate", (object?)null));

    protected override string ToString(Filter filter) => filter.ToLdapQuery();
}
