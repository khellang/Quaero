namespace Quaero;

internal sealed class FilterOptimizer : FilterTransformer
{
    public static readonly IFilterVisitor<Filter> Instance = new FilterOptimizer();

    private FilterOptimizer() { }

    public override Filter VisitAnd(AndFilter filter) => Filter.And(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitOr(OrFilter filter) => Filter.Or(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitNot(NotFilter filter)
    {
        var operand = Visit(filter.Operand);

        // not + not
        if (operand is NotFilter not)
        {
            return Visit(not.Operand);
        }

        // not + gt -> le
        // not + ge -> lt
        // not + lt -> ge
        // not + le -> gt
        if (IsComparisonFilter(operand))
        {
            return operand.Negate();
        }

        // not + eq -> ne
        // not + ne -> eq
        if (IsEqualFilter(operand))
        {
            return operand.Negate();
        }

        return Filter.Not(operand);
    }

    public override Filter VisitIn<T>(InFilter<T> filter)
    {
        if (filter.Value.Count == 1)
        {
            return VisitEqual(new EqualFilter<T>(filter.Name, filter.Value.First()));
        }

        return base.VisitIn(filter);
    }

    private static bool IsComparisonFilter(Filter filter)
    {
        var type = filter.GetType();
        if (!type.IsGenericType)
        {
            return false;
        }

        var typeDefinition = type.GetGenericTypeDefinition();
        return typeDefinition == typeof(LessThanFilter<>)
               ||typeDefinition == typeof(LessThanOrEqualFilter<>)
               ||typeDefinition == typeof(GreaterThanFilter<>)
               ||typeDefinition == typeof(GreaterThanOrEqualFilter<>);
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
