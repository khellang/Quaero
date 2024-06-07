namespace Quaero;

public sealed class LessThanOrEqualFilter<T> : PropertyValueFilter<T> where T : IComparable<T>
{
    public LessThanOrEqualFilter(string name, T value) : base("le", name, value)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitLessThanOrEqual(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitLessThanOrEqual(this);

    /// <inheritdoc />
    public override Filter Negate() => GreaterThan(Name, Value);
}
