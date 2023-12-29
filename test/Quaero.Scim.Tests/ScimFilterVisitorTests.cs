using Quaero;
using Quaero.Tests;

namespace Qaero.Scim.Tests;

public class ScimFilterVisitorTests : StringFilterVisitorTestBase
{
    [Fact]
    public Task PresenceFilter() => AssertFilter(NotEqual("disabledDate", (object?)null));

    protected override string ToString(Filter filter) => filter.ToScimFilter();
}
