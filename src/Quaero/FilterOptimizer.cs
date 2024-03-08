namespace Quaero;

internal sealed class FilterOptimizer : FilterTransformer
{
    public static readonly IFilterVisitor<Filter> Instance = new FilterOptimizer();

    private FilterOptimizer() { }

    public override Filter Visit(Filter filter) => base.Visit(NormalizeNot(filter));

    public override Filter VisitAnd(AndFilter filter) => Filter.And(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitOr(OrFilter filter) => Filter.Or(Visit(filter.Left), Visit(filter.Right));

    public override Filter VisitNot(NotFilter filter) => Filter.Not(Visit(filter.Operand));

    public override Filter VisitIn<T>(InFilter<T> filter)
    {
        if (filter.Value.Count == 1)
        {
            return VisitEqual(new EqualFilter<T>(filter.Name, filter.Value.First()));
        }

        return base.VisitIn(filter);
    }

    private static Filter NormalizeNot(Filter filter) => filter switch
    {
        AndFilter and => new AndFilter(NormalizeNot(and.Left), NormalizeNot(and.Right)),
        OrFilter or => new OrFilter(NormalizeNot(or.Left), NormalizeNot(or.Right)),
        NotFilter not => NormalizeNot(not.Operand).Negate(),
        _ => filter
    };
}
