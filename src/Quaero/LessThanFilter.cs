namespace Quaero;

public sealed class LessThanFilter<T> : PropertyValueFilter<T> where T : IComparable<T>
{
    public LessThanFilter(string name, T value) : base("lt", name, value)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitLessThan(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitLessThan(this);

    /// <inheritdoc />
    public override Filter Negate() => GreaterThanOrEqual(Name, Value);
}
