using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace Quaero.Scim;

/// <summary>
/// A visitor for converting filter expressions into SCIM query strings.
/// </summary>
public sealed class ScimFilterVisitor : StringFilterVisitor
{
    /// <summary>
    /// A singleton instance of the <see cref="ScimFilterVisitor"/>.
    /// </summary>
    public static readonly IFilterVisitor<string, StringBuilder> Instance = new ScimFilterVisitor();

    private ScimFilterVisitor() { }

    /// <inheritdoc />
    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder) =>
        VisitBinary(filter, builder);

    /// <inheritdoc />
    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder) =>
        VisitBinary(filter, builder);

    /// <inheritdoc />
    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder) =>
        VisitUnary(filter, builder);

    /// <inheritdoc />
    public override StringBuilder VisitEqual<TValue>(EqualFilter<TValue> filter, StringBuilder builder) =>
        Operator(filter, builder, "eq");

    /// <inheritdoc />
    public override StringBuilder VisitNotEqual<TValue>(NotEqualFilter<TValue> filter, StringBuilder builder)
    {
        if (filter.Value is null)
        {
            return builder.Append(filter.Name).Append(' ').Append("pr");
        }

        return Operator(filter, builder, "ne");
    }

    /// <inheritdoc />
    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        Operator(filter, builder, "sw");

    /// <inheritdoc />
    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) =>
        Operator(filter, builder, "ew");

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThan<T>(GreaterThanFilter<T> filter, StringBuilder builder) =>
        Operator(filter, builder, "gt");

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThanOrEqual<T>(GreaterThanOrEqualFilter<T> filter, StringBuilder builder) =>
        Operator(filter, builder, "ge");

    /// <inheritdoc />
    public override StringBuilder VisitLessThan<T>(LessThanFilter<T> filter, StringBuilder builder) =>
        Operator(filter, builder, "lt");

    /// <inheritdoc />
    public override StringBuilder VisitLessThanOrEqual<T>(LessThanOrEqualFilter<T> filter, StringBuilder builder) =>
        Operator(filter, builder, "le");

    private StringBuilder VisitUnary(UnaryFilter filter, StringBuilder builder) =>
        builder.Append(filter.Operator).Append('(').Append(this, filter.Operand).Append(')');

    private StringBuilder VisitBinary(BinaryFilter filter, StringBuilder builder) =>
        builder.Append('(').Append(this, filter.Left).Append(' ').Append(filter.Operator).Append(' ').Append(this, filter.Right).Append(')');

    private static StringBuilder Operator<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator) =>
        builder.Append(filter.Name).Append(' ').Append(@operator).Append(' ').Append(FormatValue(filter.Value));

    private static string FormatValue<TValue>(TValue? value) => value switch
    {
        null => "null",
        true => "true",
        false => "false",
        Guid guid => $"\"{guid}\"",
        string str => $"\"{Escape(str)}\"",
        DateTime dateTime => $"\"{dateTime:O}\"",
        DateTimeOffset dateTimeOffset => $"\"{dateTimeOffset:O}\"",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        IEnumerable<object> values => $"({string.Join(", ", values.Select(FormatValue))})",
        _ => value.ToString() ?? "null"
    };

    private static string Escape(string value) => JavaScriptEncoder.Default.Encode(value);
}
