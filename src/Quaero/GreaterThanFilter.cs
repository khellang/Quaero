namespace Quaero;

public sealed class GreaterThanFilter<T> : PropertyValueFilter<T> where T : IComparable<T>
{
    public GreaterThanFilter(string name, T value) : base("gt", name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitGreaterThan(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitGreaterThan(this);

    public override Filter Negate() => LessThanOrEqual(Name, Value);
}
