namespace Quaero.Tests;

[UsesVerify]
public abstract class StringFilterVisitorTestBase : FilterVisitorTestBase
{
    protected override Task AssertFilter(Filter filter)
    {
        var settingsTask = Verify(ToString(filter.Optimize()));

        if (filter is PropertyFilter<object> propertyFilter)
        {
            // For parameterized tests, we need to specify parameters to disambiguate file names.
            settingsTask.UseParameters(propertyFilter.Name, propertyFilter.Value);
        }

        return settingsTask;
    }

    protected abstract string ToString(Filter filter);
}
