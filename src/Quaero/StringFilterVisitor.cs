using System.Text;

namespace Quaero;

public abstract class StringFilterVisitor : IFilterVisitor<string, StringBuilder>
{
    public string Visit(Filter filter)
    {
        var builder = StringBuilderCache.Acquire();
        builder = filter.Accept(this, builder);
        return StringBuilderCache.GetStringAndRelease(builder);
    }

    public abstract StringBuilder VisitAnd(AndFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitOr(OrFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitNot(NotFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitEqual<T>(EqualFilter<T> filter, StringBuilder builder);

    public abstract StringBuilder VisitNotEqual<T>(NotEqualFilter<T> filter, StringBuilder builder);

    public abstract StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitGreaterThan(GreaterThanFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitLessThan(LessThanFilter filter, StringBuilder builder);

    public abstract StringBuilder VisitLessThanOrEqual(LessThanOrEqualFilter filter, StringBuilder builder);
}
