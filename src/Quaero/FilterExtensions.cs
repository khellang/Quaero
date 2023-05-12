namespace Quaero;

public static class FilterExtensions
{
    public static Filter Optimize(this Filter filter) => 
        FilterOptimizer.Optimize(filter);
}
