namespace Quaero;

/// <summary>
/// A convenience base class for filter transformers where both the input and output is a <see cref="Filter"/>.
/// </summary>
public abstract class FilterTransformer : IFilterVisitor<Filter>
{
    /// <inheritdoc />
    public virtual Filter Visit(Filter filter) => filter.Accept(this);

    /// <inheritdoc />
    public virtual Filter VisitAnd(AndFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitOr(OrFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitNot(NotFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitEqual<T>(EqualFilter<T> filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitNotEqual<T>(NotEqualFilter<T> filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitStartsWith(StartsWithFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitEndsWith(EndsWithFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitContains(ContainsFilter filter) => filter;

    /// <inheritdoc />
    public virtual Filter VisitGreaterThan<T>(GreaterThanFilter<T> filter) where T : IComparable<T> => filter;

    /// <inheritdoc />
    public virtual Filter VisitGreaterThanOrEqual<T>(GreaterThanOrEqualFilter<T> filter) where T : IComparable<T> => filter;

    /// <inheritdoc />
    public virtual Filter VisitLessThan<T>(LessThanFilter<T> filter) where T : IComparable<T> => filter;

    /// <inheritdoc />
    public virtual Filter VisitLessThanOrEqual<T>(LessThanOrEqualFilter<T> filter) where T : IComparable<T> => filter;

    /// <inheritdoc />
    public virtual Filter VisitIn<T>(InFilter<T> filter) => filter;
}
