namespace Quaero;

/// <summary>
/// A filter representing the function to check whether a string ends with a value.
/// </summary>
public sealed class EndsWithFilter : PropertyFilter<string?>
{
    /// <summary>
    /// Constructs a new instance of an <see cref="EndsWithFilter"/>.
    /// </summary>
    /// <param name="name">The name of the property to check.</param>
    /// <param name="value">The value to check for.</param>
    public EndsWithFilter(string name, string? value) : base(name, value) { }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitEndsWith(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitEndsWith(this);

    /// <inheritdoc />
    public override string ToString() => $"{Name} endsWith {FormatValue(Value)}";
}
