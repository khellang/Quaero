namespace Quaero;

/// <summary>
/// A base class representing binary logical operators.
/// </summary>
public abstract class BinaryFilter : Filter
{
    /// <summary>
    /// Constructs a new instance of a <see cref="BinaryFilter"/> with a
    /// <paramref name="left"/> and a <paramref name="right"/> operand.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="operator"/>, <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.</exception>
    protected BinaryFilter(string @operator, Filter left, Filter right)
    {
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// The operator.
    /// </summary>
    public string Operator { get; }

    /// <summary>
    /// The left operand.
    /// </summary>
    public Filter Left { get; }

    /// <summary>
    /// The right operand.
    /// </summary>
    public Filter Right { get; }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} {Operator} {Right})";
}
