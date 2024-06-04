namespace Quaero.Tests;

public abstract class StringFilterVisitorTestBase : FilterVisitorTestBase
{
    protected override Task AssertFilter(Filter filter)
    {
        var settingsTask = Verify(ToString(filter.Optimize()));

        if (filter is PropertyValueFilter<object> propertyFilter)
        {
            // For parameterized tests, we need to specify parameters to disambiguate file names.
            settingsTask.UseParameters(propertyFilter.Name, propertyFilter.Value);
        }

        return settingsTask;
    }

    protected abstract string ToString(Filter filter);
}
