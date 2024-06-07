namespace Quaero;

/// <summary>
/// A filter representing the binary logical OR operator.
/// </summary>
public sealed class OrFilter : BinaryFilter
{
    /// <summary>
    /// Constructs a new instance of an <see cref="OrFilter"/>.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public OrFilter(Filter left, Filter right) : base("or", left, right)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitOr(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitOr(this);

    /// <summary>
    /// Negates the OR expression, producing an AND expression.
    /// </summary>
    /// <returns>The negated OR expression as an AND expression.</returns>
    public override Filter Negate() => Left.Negate().And(Right.Negate());
}
