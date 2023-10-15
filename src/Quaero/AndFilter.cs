namespace Quaero;

/// <summary>
/// A filter representing the binary logical AND operator.
/// </summary>
public sealed class AndFilter : BinaryFilter
{
    /// <summary>
    /// Constructs a new instance of an <see cref="AndFilter"/>.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public AndFilter(Filter left, Filter right) : base(left, right)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitAnd(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitAnd(this);

    /// <summary>
    /// Negates the AND expression, producing an OR expression.
    /// </summary>
    /// <returns>The negated AND expression as an OR expression.</returns>
    public override Filter Negate() => Or(Left.Negate(), Right.Negate());

    /// <inheritdoc />
    public override string ToString() => $"({Left} and {Right})";
}
