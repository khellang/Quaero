namespace Quaero;

/// <summary>
/// A base class for filters operating on a property and its value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public abstract class PropertyValueFilter<T> : PropertyFilter, IEquatable<PropertyValueFilter<T>?>
{
    /// <summary>
    /// Constructs a new instance of a <see cref="PropertyValueFilter{T}"/>.
    /// </summary>
    /// <param name="operator">The filter operator.</param>
    /// <param name="name">The name of the property to check.</param>
    /// <param name="value">The value to check for.</param>
    protected PropertyValueFilter(string @operator, string name, T value) : base(@operator, name)
    {
        Value = value;
    }

    /// <summary>
    /// The value to check for.
    /// </summary>
    public T Value { get; }

    /// <inheritdoc />
    public override bool Equals(Filter? other) => Equals(other as PropertyValueFilter<T>);

    /// <inheritdoc/>
    public virtual bool Equals(PropertyValueFilter<T>? other) =>
        base.Equals(other) &&
            StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name) &&
               EqualityComparer<T>.Default.Equals(Value, other.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), StringComparer.OrdinalIgnoreCase.GetHashCode(Name), Value);

    /// <inheritdoc/>
    public override string ToString() => $"{Name} {Operator} {FormatValue(Value)}";
}
