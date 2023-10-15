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
    public abstract StringBuilder VisitGreaterThan(GreaterThanFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitLessThan(LessThanFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitLessThanOrEqual(LessThanOrEqualFilter filter, StringBuilder builder);

    /// <inheritdoc />
    public abstract StringBuilder VisitIn<T>(InFilter<T> filter, StringBuilder state);
}
