namespace Quaero;

public sealed class NotEqualFilter<T> : PropertyValueFilter<T?>
{
    public NotEqualFilter(string name, T? value) : base("ne", name, value) { }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitNotEqual(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitNotEqual(this);

    /// <inheritdoc />
    public override Filter Negate() => Equal(Name, Value);
}
