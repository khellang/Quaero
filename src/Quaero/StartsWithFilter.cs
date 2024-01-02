namespace Quaero;

/// <summary>
/// A filter representing the function to check whether a string ends with a value.
/// </summary>
public sealed class StartsWithFilter : PropertyFilter<string?>
{
    /// <summary>
    /// Constructs a new instance of an <see cref="StartsWithFilter"/>.
    /// </summary>
    /// <param name="name">The name of the property to check.</param>
    /// <param name="value">The value to check for.</param>
    public StartsWithFilter(string name, string? value) : base("sw", name, value) { }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitStartsWith(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitStartsWith(this);
}
