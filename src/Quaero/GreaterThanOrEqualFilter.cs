namespace Quaero;

public sealed class GreaterThanOrEqualFilter<T> : PropertyValueFilter<T> where T : IComparable<T>
{
    public GreaterThanOrEqualFilter(string name, T value) : base("ge", name, value)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitGreaterThanOrEqual(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitGreaterThanOrEqual(this);

    /// <inheritdoc />
    public override Filter Negate() => LessThan(Name, Value);
}
