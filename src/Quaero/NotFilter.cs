namespace Quaero;

/// <summary>
/// A filter representing the unary logical NOT operator.
/// </summary>
public sealed class NotFilter : Filter
{
    /// <summary>
    /// Constructs a new instance of an <see cref="NotFilter"/>.
    /// </summary>
    /// <param name="operand">The operand to wrap in a NOT operator.</param>
    public NotFilter(Filter operand)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    /// <summary>
    /// The wrapped operand.
    /// </summary>
    public Filter Operand { get; }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitNot(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitNot(this);

    /// <summary>
    /// Negates the filter by removing the wrapping NOT operator.
    /// </summary>
    /// <returns>A negated filter witout a wrapping NOT operator.</returns>
    public override Filter Negate() => Operand;

    /// <inheritdoc />
    public override string ToString() => $"not({Operand})";
}
