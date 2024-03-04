namespace Quaero;

/// <summary>
/// A filter representing the function to check whether a string contains a value.
/// </summary>
public sealed class ContainsFilter : PropertyValueFilter<string?>
{
    /// <summary>
    /// Constructs a new instance of an <see cref="ContainsFilter"/>.
    /// </summary>
    /// <param name="name">The name of the property to check.</param>
    /// <param name="value">The value to check for.</param>
    public ContainsFilter(string name, string? value) : base("co", name, value) { }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitContains(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitContains(this);
}