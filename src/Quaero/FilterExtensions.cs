namespace Quaero;

/// <summary>
/// Various filter extensions.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Optimizes the specified <paramref name="filter"/> by simplifying operators, removing double negation etc.
    /// </summary>
    /// <param name="filter">The filter to optimize.</param>
    /// <returns>An optimized version of the specified <paramref name="filter"/></returns>
    public static Filter Optimize(this Filter filter) =>
        FilterOptimizer.Instance.Visit(filter);

    /// <summary>
    /// Converts the specified <paramref name="filter"/> to an in-memory predicate,
    /// using reflection to get property values from <typeparamref name="T"/>.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    /// <typeparam name="T">The type the predicate will operate on.</typeparam>
    /// <returns>An in-memory predicate based on the specified <paramref name="filter"/>.</returns>
    public static Func<T, bool> ToReflectionBasedPredicate<T>(this Filter filter) =>
        filter.ToPredicate(InMemoryFilterVisitor<T>.ReflectionBased);

    /// <summary>
    /// Converts the specified <paramref name="filter"/> to an in-memory predicate,
    /// using the specified <paramref name="visitor"/> for conversion.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    /// <param name="visitor">The visitor to use for conversion.</param>
    /// <typeparam name="T">The type the predicate will operate on.</typeparam>
    /// <returns>An in-memory predicate based on the specified <paramref name="filter"/>.</returns>
    public static Func<T, bool> ToPredicate<T>(this Filter filter, InMemoryFilterVisitor<T> visitor) =>
        visitor.Visit(filter);
}
