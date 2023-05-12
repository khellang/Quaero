namespace Quaero;

public sealed class LessThanFilter : PropertyFilter<IComparable>
{
    public LessThanFilter(string name, IComparable value) : base(name, value)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitLessThan(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitLessThan(this);

    public override Filter Negate() => GreaterThanOrEqual(Name, Value);

    public override string ToString() => $"{Name} lt {FormatValue(Value)}";
}
