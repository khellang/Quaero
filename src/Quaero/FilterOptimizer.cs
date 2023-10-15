namespace Quaero;

internal sealed class FilterOptimizer : FilterTransformer
{
    public static readonly IFilterVisitor<Filter> Instance = new FilterOptimizer();

    private FilterOptimizer() { }

    public override Filter VisitAnd(AndFilter filter) => Filter.And(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitOr(OrFilter filter) => Filter.Or(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitNot(NotFilter filter)
    {
        // not + not
        if (filter.Operand is NotFilter not)
        {
            return Visit(not.Operand);
        }

        // not + gt -> le
        if (filter.Operand is GreaterThanFilter gt)
        {
            return gt.Negate();
        }

        // not + ge -> lt
        if (filter.Operand is GreaterThanOrEqualFilter ge)
        {
            return ge.Negate();
        }

        // not + lt -> ge
        if (filter.Operand is LessThanFilter lt)
        {
            return lt.Negate();
        }

        // not + le -> gt
        if (filter.Operand is LessThanOrEqualFilter le)
        {
            return le.Negate();
        }

        // not + eq -> ne
        // not + ne -> eq
        if (IsEqualFilter(filter.Operand))
        {
            return filter.Operand.Negate();
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
