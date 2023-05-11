namespace Quaero;

public sealed class FilterOptimizer : FilterVisitor
{
    public static readonly IFilterVisitor<Filter> Instance = new FilterOptimizer();

    private FilterOptimizer() { }

    public static Filter Optimize(Filter filter) => Instance.Visit(filter);

    public override Filter VisitAnd(AndFilter filter) => Filter.And(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitOr(OrFilter filter) => Filter.Or(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitNot(NotFilter filter)
    {
        // not + not
        if (filter.Inner is NotFilter not)
        {
            return Visit(not.Inner);
        }

        // not + gt -> le
        if (filter.Inner is GreaterThanFilter gt)
        {
            return gt.Negate();
        }

        // not + ge -> lt
        if (filter.Inner is GreaterThanOrEqualFilter ge)
        {
            return ge.Negate();
        }

        // not + lt -> ge
        if (filter.Inner is LessThanFilter lt)
        {
            return lt.Negate();
        }

        // not + le -> gt
        if (filter.Inner is LessThanOrEqualFilter le)
        {
            return le.Negate();
        }

        // not + eq -> ne
        // not + ne -> eq
        if (IsEqualFilter(filter.Inner))
        {
            return filter.Inner.Negate();
        }

        return filter;
    }

    private static bool IsEqualFilter(Filter filter)
    {
        var type = filter.GetType();
        if (!type.IsGenericType)
        {
            return false;
        }

        var typeDefinition = type.GetGenericTypeDefinition();
        return typeDefinition == typeof(EqualFilter<>)
               ||typeDefinition == typeof(NotEqualFilter<>);
    }
}
