namespace Quaero;

public sealed class EqualFilter<T> : PropertyFilter<T?>
{
    public EqualFilter(string name, T? value) : base("eq", name, value) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitEqual(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitEqual(this);

    public override Filter Negate() => NotEqual(Name, Value);
}
