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
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.</exception>
    protected BinaryFilter(Filter left, Filter right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// The left operand.
    /// </summary>
    public Filter Left { get; }

    /// <summary>
    /// The right operand.
    /// </summary>
    public Filter Right { get; }
}
