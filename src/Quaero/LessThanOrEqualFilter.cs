namespace Quaero;

public sealed class LessThanOrEqualFilter : PropertyFilter<IComparable>
{
    public LessThanOrEqualFilter(string name, IComparable value) : base(name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitLessThanOrEqual(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitLessThanOrEqual(this);

    public override Filter Negate() => GreaterThan(Name, Value);

    public override string ToString() => $"{Name} le {FormatValue(Value)}";
}
