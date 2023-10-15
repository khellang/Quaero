namespace Quaero;

/// <summary>
/// A base class for filters operating on a property and its value.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PropertyFilter<T> : Filter
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
}
