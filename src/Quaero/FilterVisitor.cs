namespace Quaero;

public abstract class FilterVisitor : IFilterVisitor<Filter>
{
    public virtual Filter Visit(Filter filter) => filter.Accept(this);

    public virtual Filter VisitAnd(AndFilter filter) => filter;

    public virtual Filter VisitOr(OrFilter filter) => filter;

    public virtual Filter VisitNot(NotFilter filter) => filter;

    public virtual Filter VisitEqual<T>(EqualFilter<T> filter) => filter;

    public virtual Filter VisitNotEqual<T>(NotEqualFilter<T> filter) => filter;

    public virtual Filter VisitStartsWith(StartsWithFilter filter) => filter;

    public virtual Filter VisitEndsWith(EndsWithFilter filter) => filter;

    public virtual Filter VisitGreaterThan(GreaterThanFilter filter) => filter;

    public virtual Filter VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter) => filter;

    public virtual Filter VisitLessThan(LessThanFilter filter) => filter;

    public virtual Filter VisitLessThanOrEqual(LessThanOrEqualFilter filter) => filter;
}
