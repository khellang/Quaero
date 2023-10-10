namespace Quaero;

public static class FilterExtensions
{
    public static Filter Optimize(this Filter filter) =>
        FilterOptimizer.Optimize(filter);

    public static Func<T, bool> ToReflectionBasedPredicate<T>(this Filter filter) =>
        filter.ToPredicate(InMemoryFilterVisitor<T>.ReflectionBased);

    public static Func<T, bool> ToPredicate<T>(this Filter filter, InMemoryFilterVisitor<T> visitor) =>
        visitor.Visit(filter);
}
