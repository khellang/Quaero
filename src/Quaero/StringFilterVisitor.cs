using System.Text;

namespace Quaero;

/// <summary>
/// A convenience base class for filter visitors producing a <see cref="string"/> result.
/// </summary>
public abstract class StringFilterVisitor : IFilterVisitor<string, StringBuilder>
{
    /// <inheritdoc />
    public string Visit(Filter filter)
    {
        var builder = StringBuilderCache.Acquire();
        builder = filter.Accept(this, builder);
        return StringBuilderCache.GetStringAndRelease(builder);
    }

    /// <inheritdoc />
    public abstract StringBuilder VisitAnd(AndFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitOr(OrFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitNot(NotFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitEqual<T>(EqualFilter<T> filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitNotEqual<T>(NotEqualFilter<T> filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitGreaterThan<T>(GreaterThanFilter<T> filter, StringBuilder builder)
        where T : IComparable<T>;

    /// <inheritdoc />
    public abstract StringBuilder VisitGreaterThanOrEqual<T>(GreaterThanOrEqualFilter<T> filter, StringBuilder builder)
        where T : IComparable<T>;

    /// <inheritdoc />
    public abstract StringBuilder VisitLessThan<T>(LessThanFilter<T> filter, StringBuilder builder)
        where T : IComparable<T>;

    /// <inheritdoc />
    public abstract StringBuilder VisitLessThanOrEqual<T>(LessThanOrEqualFilter<T> filter, StringBuilder builder)
        where T : IComparable<T>;

    /// <inheritdoc />
    public virtual StringBuilder VisitIn<T>(InFilter<T> filter, StringBuilder state) =>
        filter.Value
            .Select(x => Filter.Equal(filter.Name, x))
            .Aggregate(Filter.Or)
            .Accept(this, state);
}
