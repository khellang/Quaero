namespace Quaero;

/// <summary>
/// A base class for filters operating on a property.
/// </summary>
public abstract class PropertyFilter : Filter, IEquatable<PropertyFilter>
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Constructs a new instance of a <see cref="PropertyFilter"/>.
    /// </summary>
    /// <param name="operator">The filter operator.</param>
    /// <param name="name">The name of the property to check.</param>
    protected PropertyFilter(string @operator, string name) : base(@operator)
    {
        Name = name;
    }

    /// <summary>
    /// The name of the property to check.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public override bool Equals(Filter? other) => Equals(other as PropertyFilter);

    /// <inheritdoc />
    public bool Equals(PropertyFilter? other) => base.Equals(other) && Comparer.Equals(Name, other.Name);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Comparer.GetHashCode(Name));
}
