namespace Quaero;

public sealed class GreaterThanOrEqualFilter<T> : PropertyFilter<T> where T : IComparable<T>
{
    public GreaterThanOrEqualFilter(string name, T value) : base("ge", name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitGreaterThanOrEqual(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitGreaterThanOrEqual(this);

    public override Filter Negate() => LessThan(Name, Value);
}
