using Quaero.Tests;

namespace Quaero.Ldap.Tests;

public class LdapFilterVisitorTests : StringFilterVisitorTestBase
{
    [Fact]
    public Task Absencefilter() => AssertFilter(Equal("disabledDate", (object?)null));

    [Fact]
    public Task PresenceFilter() => AssertFilter(NotEqual("disabledDate", (object?)null));

    protected override string ToString(Filter filter) => filter.ToLdapQuery();
}
