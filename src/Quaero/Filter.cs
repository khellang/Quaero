using System.Globalization;
using Superpower;

namespace Quaero;

/// <summary>
/// The base class for all filter expressions.
/// </summary>
public abstract class Filter
{
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

    public static Filter GreaterThan(string name, IComparable value) => new GreaterThanFilter(name, value);

    public static Filter GreaterThanOrEqual(string name, IComparable value) => new GreaterThanOrEqualFilter(name, value);

    public static Filter LessThan(string name, IComparable value) => new LessThanFilter(name, value);

    public static Filter LessThanOrEqual(string name, IComparable value) => new LessThanOrEqualFilter(name, value);

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

    public static Filter Parse(string filter)
    {
        var tokens = FilterTokenizer.Instance.Tokenize(filter);
        return FilterParser.Instance.Parse(tokens);
    }

    public abstract TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state);

    public abstract TResult Accept<TResult>(IFilterVisitor<TResult> visitor);

    /// <summary>
    /// Converts the filter to a <see cref="string"/>.
    /// </summary>
    /// <returns>A formatted <see cref="string"/>, representing the filter expression.</returns>
    public abstract override string ToString();

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
        string str => $"'{Escape(str)}'",
        DateTime dateTime => dateTime.ToString("O"),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        IEnumerable<object> values => $"({string.Join(", ", values.Select(FormatValue))})",
        _ => value.ToString() ?? "null"
    };

    private static string Escape(string value) => value.Replace("'", "''");
}
