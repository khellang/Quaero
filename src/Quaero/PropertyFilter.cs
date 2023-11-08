namespace Quaero;

/// <summary>
/// A base class for filters operating on a property and its value.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PropertyFilter<T> : Filter, IEquatable<PropertyFilter<T>?>
{
    /// <summary>
    /// Constructs a new instance of a <see cref="PropertyFilter{T}"/>.
    /// </summary>
    /// <param name="name">The name of the property to check.</param>
    /// <param name="value">The value to check for.</param>
    protected PropertyFilter(string name, T value) => (Name, Value) = (name, value);

    /// <summary>
    /// The name of the property to check.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The value to check for.
    /// </summary>
    public T Value { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as PropertyFilter<T>);

    /// <inheritdoc/>
    public bool Equals(PropertyFilter<T>? other) =>
        other is not null &&
            StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name) &&
               EqualityComparer<T>.Default.Equals(Value, other.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(Name), Value);
}
