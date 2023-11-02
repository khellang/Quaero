using Quaero;
using Quaero.Tests;

namespace Qaero.MicrosoftGraph.Tests;

public class MicrosoftGraphFilterVisitorTests : StringFilterVisitorTestBase
{
    protected override string ToString(Filter filter) => filter.ToMicrosoftGraphFilter();
}
