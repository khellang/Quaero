namespace Quaero.Tests;

[UsesVerify]
public abstract class StringFilterVisitorTestBase
{
    [Fact]
    public void EqualFilter() => Verify(Convert(Equal("name", "John Doe")));

    protected abstract string Convert(Filter filter);
}
