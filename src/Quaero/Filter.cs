using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Superpower;

namespace Quaero;

/// <summary>
/// The base class for all filter expressions.
/// </summary>
public abstract class Filter : IEquatable<Filter>
{
    protected Filter(string @operator)
    {
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
    }

    /// <summary>
    /// The filter operator.
    /// </summary>
    protected string Operator { get; }

    /// <summary>
    /// Convenience method to construct an <see name="EqualFilter{T}"/>.
    /// </summary>
    /// <param name="name">The name of the property to check for equality.</param>
    /// <param name="value">The value to check for equality.</param>
    /// <typeparam name="T">The type of the property to check for equality.</typeparam>
    /// <returns>An <see cref="EqualFilter{T}"/> with the specified <paramref name="name"/> and <paramref name="value"/>.</returns>
    public static Filter Equal<T>(string name, T? value) => new EqualFilter<T>(name, value);

    /// <summary>
    /// Convenience method to construct a <see name="NotEqualFilter{T}"/>.
    /// </summary>
    /// <param name="name">The name of the property to check for inequality.</param>
    /// <param name="value">The value to check for inequality.</param>
    /// <typeparam name="T">The type of the property to check for inequality.</typeparam>
    /// <returns>A <see cref="NotEqualFilter{T}"/> with the specified <paramref name="name"/> and <paramref name="value"/>.</returns>
    public static Filter NotEqual<T>(string name, T? value) => new NotEqualFilter<T>(name, value);

    /// <summary>
    /// Convenience method to construct a <see name="InFilter{T}"/>.
    /// </summary>
    /// <param name="name">The name of the property to check for inequality.</param>
    /// <param name="values">The values to check for equality.</param>
    /// <typeparam name="T">The type of the property to check for inequality.</typeparam>
    /// <returns>A <see cref="InFilter{T}"/> with the specified <paramref name="name"/> and <paramref name="values"/>.</returns>
    public static Filter In<T>(string name, params T[] values) => new InFilter<T>(name, new HashSet<T>(values));

    public static Filter StartsWith(string name, string? value) => new StartsWithFilter(name, value);

    public static Filter EndsWith(string name, string? value) => new EndsWithFilter(name, value);

    public static Filter Contains(string name, string? value) => new ContainsFilter(name, value);

    public static Filter GreaterThan<T>(string name, T value) where T : IComparable<T> =>
        new GreaterThanFilter<T>(name, value);

    public static Filter GreaterThanOrEqual<T>(string name, T value) where T : IComparable<T> =>
        new GreaterThanOrEqualFilter<T>(name, value);

    public static Filter LessThan<T>(string name, T value) where T : IComparable<T> =>
        new LessThanFilter<T>(name, value);

    public static Filter LessThanOrEqual<T>(string name, T value) where T : IComparable<T> =>
        new LessThanOrEqualFilter<T>(name, value);

    public static Filter Present(string name) => new PresenceFilter(name);

    public static Filter Not(Filter filter) => new NotFilter(filter);

    public static Filter And(Filter left, Filter right) => new AndFilter(left, right);

    public static Filter Or(Filter left, Filter right) => new OrFilter(left, right);

    public Filter And(Filter other) => And(this, other);

    public Filter Or(Filter other) => Or(this, other);

    /// <summary>
    /// Negates the filter.
    /// </summary>
    /// <returns>A negated version of the filter.</returns>
    public virtual Filter Negate() => Not(this);

    /// <summary>
    /// Parses <paramref name="filter"/> into a <see cref="Filter"/> expression.
    /// </summary>
    /// <param name="filter">The filter string to parse.</param>
    /// <returns>A parsed <see cref="Filter"/> expression.</returns>
    public static Filter Parse(string filter)
    {
        var tokens = FilterTokenizer.Instance.Tokenize(filter);
        return FilterParser.Instance.Parse(tokens);
    }

    /// <summary>
    /// Attempts to parse <paramref name="filter"/> into a <see cref="Filter"/> expression.
    /// </summary>
    /// <param name="filter">The filter string to parse.</param>
    /// <param name="result">A parsed <see cref="Filter"/> expression if successful.</param>
    /// <returns><c>true</c> if the filter was successfully parsed, otherwise <c>false</c></returns>
    public static bool TryParse(string filter, [NotNullWhen(true)] out Filter? result)
    {
        var tokens = FilterTokenizer.Instance.TryTokenize(filter);
        if (tokens.HasValue)
        {
            var parseResult = FilterParser.Instance.TryParse(tokens.Value);
            if (parseResult.HasValue)
            {
                result = parseResult.Value;
                return true;
            }
        }

        result = default;
        return false;
    }

    public abstract TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state);

    public abstract TResult Accept<TResult>(IFilterVisitor<TResult> visitor);

    /// <summary>
    /// Converts the filter to a <see cref="string"/>.
    /// </summary>
    /// <returns>A formatted <see cref="string"/>, representing the filter expression.</returns>
    public abstract override string ToString();

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Filter);

    /// <inheritdoc />
    public virtual bool Equals(Filter? other)
    {
        if (other is null)
        {
            return false;
        }

        if (other.GetType() != GetType())
        {
            return false;
        }

        return Operator == other.Operator;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Operator.GetHashCode();

    /// <summary>
    /// Formats the specified <paramref name="value"/> into a <see cref="string"/>.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <typeparam name="TValue">The type of value to format.</typeparam>
    /// <returns>A formatted <see cref="string"/>, representing the <paramref name="value"/>.</returns>
    protected static string FormatValue<TValue>(TValue value) => value switch {
        null => "null",
        true => "true",
        false => "false",
        Guid guid => $"\"{guid}\"",
        string str => $"\"{Escape(str)}\"",
        DateTime dateTime => $"\"{dateTime:O}\"",
        DateTimeOffset dateTimeOffset => $"\"{dateTimeOffset:O}\"",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        IEnumerable<object> values => $"[{string.Join(", ", values.Select(FormatValue))}]",
        _ => value.ToString() ?? "null"
    };

    private static string Escape(string value) => value.Replace("\"", "\\\"");
}
