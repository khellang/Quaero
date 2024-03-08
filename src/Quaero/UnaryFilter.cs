namespace Quaero;

/// <summary>
/// Represents a base class for all unary operators.
/// </summary>
public abstract class UnaryFilter : Filter, IEquatable<UnaryFilter>
{
    /// <summary>
    /// Constructs a new instance of an <see cref="UnaryFilter"/>.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="operand">The operand.</param>
    public UnaryFilter(string @operator, Filter operand) : base(@operator)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    /// <summary>
    /// The operand.
    /// </summary>
    public Filter Operand { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Operator}({Operand})";

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as UnaryFilter);

    /// <inheritdoc />
    public virtual bool Equals(UnaryFilter? other) =>
        base.Equals(other) && Operand.Equals(other.Operand);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Operand);
}
