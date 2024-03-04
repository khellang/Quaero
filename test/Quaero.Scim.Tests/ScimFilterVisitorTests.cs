using Quaero.Tests;

namespace Quaero.Scim.Tests;

public class ScimFilterVisitorTests : StringFilterVisitorTestBase
{
    protected override string ToString(Filter filter) => filter.ToScimFilter();
}
