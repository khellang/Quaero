namespace Quaero;

public sealed class LessThanFilter<T> : PropertyFilter<T> where T : IComparable<T>
{
    public LessThanFilter(string name, T value) : base("lt", name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitLessThan(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitLessThan(this);

    public override Filter Negate() => GreaterThanOrEqual(Name, Value);
}
