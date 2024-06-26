namespace Quaero;

public sealed class EqualFilter<T> : PropertyValueFilter<T?>
{
    public EqualFilter(string name, T? value) : base("eq", name, value) { }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitEqual(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitEqual(this);

    /// <inheritdoc />
    public override Filter Negate() => NotEqual(Name, Value);
}
