namespace Quaero;

public sealed class NotEqualFilter<T> : PropertyFilter<T?>
{
    public NotEqualFilter(string name, T? value) : base(name, value) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitNotEqual(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitNotEqual(this);

    public override Filter Negate() => Equal(Name, Value);

    public override string ToString() => $"{Name} ne {FormatValue(Value)}";
}
