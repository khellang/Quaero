namespace Quaero;

internal sealed class FilterOptimizer : FilterTransformer
{
    public static readonly IFilterVisitor<Filter> Instance = new FilterOptimizer();

    private FilterOptimizer() { }

    public override Filter Visit(Filter filter) => base.Visit(NormalizeNot(filter));

    public override Filter VisitAnd(AndFilter filter)
    {
        var left = Visit(filter.Left);
        var right = Visit(filter.Right);

        if (left.Equals(right))
        {
            return left;
        }

        if (left is NotFilter leftNot && right is NotFilter rightNot)
        {
            // DeMorgan simplification:
            // (not A) and (not B) => not (A or B)
            return Filter.Not(leftNot.Operand.Or(rightNot.Operand));
        }

        return left.And(right);
    }

    public override Filter VisitOr(OrFilter filter)
    {
        var left = Visit(filter.Left);
        var right = Visit(filter.Right);

        if (left.Equals(right))
        {
            return left;
        }

        if (left is NotFilter leftNot && right is NotFilter rightNot)
        {
            // DeMorgan simplification:
            // (not A) or (not B) => not (A and B)
            return Filter.Not(leftNot.Operand.And(rightNot.Operand));
        }

        return left.Or(right);
    }

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
        AndFilter and => NormalizeNot(and.Left).And(NormalizeNot(and.Right)),
        OrFilter or => NormalizeNot(or.Left).Or(NormalizeNot(or.Right)),
        NotFilter not => NormalizeNot(not.Operand).Negate(),
        _ => filter
    };
}
