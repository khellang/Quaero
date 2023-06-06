namespace Quaero;

public sealed class GreaterThanOrEqualFilter : PropertyFilter<IComparable>
{
    public GreaterThanOrEqualFilter(string name, IComparable value) : base(name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitGreaterThanOrEqual(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitGreaterThanOrEqual(this);

    public override Filter Negate() => LessThan(Name, Value);

    public override string ToString() => $"{Name} ge {FormatValue(Value)}";
}
