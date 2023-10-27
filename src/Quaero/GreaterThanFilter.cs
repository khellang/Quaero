namespace Quaero;

public sealed class GreaterThanFilter<T> : PropertyFilter<T> where T : IComparable<T>
{
    public GreaterThanFilter(string name, T value) : base(name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitGreaterThan(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitGreaterThan(this);

    public override Filter Negate() => LessThanOrEqual(Name, Value);

    public override string ToString() => $"{Name} gt {FormatValue(Value)}";
}
