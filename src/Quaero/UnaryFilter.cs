namespace Quaero;

/// <summary>
/// Represents a base class for all unary operators.
/// </summary>
public abstract class UnaryFilter : Filter
{
    /// <summary>
    /// Constructs a new instance of an <see cref="UnaryFilter"/>.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="operand">The operand.</param>
    public UnaryFilter(string @operator, Filter operand)
    {
        Operator = @operator;
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public string Operator { get; }

    /// <summary>
    /// The operand.
    /// </summary>
    public Filter Operand { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Operator}({Operand})";
}
