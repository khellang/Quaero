namespace Quaero;

/// <summary>
/// A filter representing the unary logical NOT operator.
/// </summary>
public sealed class NotFilter : UnaryFilter
{
    /// <summary>
    /// Constructs a new instance of an <see cref="NotFilter"/>.
    /// </summary>
    /// <param name="operand">The operand to wrap in a NOT operator.</param>
    public NotFilter(Filter operand) : base("not", operand)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitNot(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitNot(this);

    /// <summary>
    /// Negates the filter by removing the wrapping NOT operator.
    /// </summary>
    /// <returns>A negated filter without a wrapping NOT operator.</returns>
    public override Filter Negate() => Operand;
}
