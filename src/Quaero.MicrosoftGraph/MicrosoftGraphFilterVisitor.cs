using System.Globalization;
using System.Text;

namespace Quaero.MicrosoftGraph;

/// <summary>
/// A visitor for converting filter expressions into Microsoft Graph query strings.
/// </summary>
public sealed class MicrosoftGraphFilterVisitor : StringFilterVisitor
{
    /// <summary>
    /// A singleton instance of the <see cref="MicrosoftGraphFilterVisitor"/>.
    /// </summary>
    public static readonly IFilterVisitor<string, StringBuilder> Instance = new MicrosoftGraphFilterVisitor();

    private MicrosoftGraphFilterVisitor() { }

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
        InfixOperator(filter, builder, "eq");

    /// <inheritdoc />
    public override StringBuilder VisitNotEqual<TValue>(NotEqualFilter<TValue> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "ne");

    /// <inheritdoc />
    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        PrefixOperator(filter, builder, "startsWith");

    /// <inheritdoc />
    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) =>
        PrefixOperator(filter, builder, "endsWith");

    /// <inheritdoc />
    public override StringBuilder VisitContains(ContainsFilter filter, StringBuilder builder) =>
        PrefixOperator(filter, builder, "contains");

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThan<T>(GreaterThanFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "gt");

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThanOrEqual<T>(GreaterThanOrEqualFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "ge");

    /// <inheritdoc />
    public override StringBuilder VisitLessThan<T>(LessThanFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "lt");

    /// <inheritdoc />
    public override StringBuilder VisitLessThanOrEqual<T>(LessThanOrEqualFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "le");

    /// <inheritdoc />
    public override StringBuilder VisitIn<T>(InFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "in");

    private StringBuilder VisitUnary(UnaryFilter filter, StringBuilder builder) =>
        builder.Append(filter.Operator).Append('(').Append(this, filter.Operand).Append(')');

    private StringBuilder VisitBinary(BinaryFilter filter, StringBuilder builder) =>
        builder.Append('(').Append(this, filter.Left).Append(' ').Append(filter.Operator).Append(' ').Append(this, filter.Right).Append(')');

    private static StringBuilder InfixOperator<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator) =>
        builder.Append(filter.Name).Append(' ').Append(@operator).Append(' ').Append(FormatValue(filter.Value));

    private static StringBuilder PrefixOperator<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator) =>
        builder.Append(@operator).Append('(').Append(filter.Name).Append(", ").Append(FormatValue(filter.Value)).Append(')');

    private static string FormatValue<TValue>(TValue? value) => value switch
    {
        null => "null",
        true => "true",
        false => "false",
        Guid guid => $"'{guid}'",
        string str => $"'{Escape(str)}'",
        DateTime dateTime => dateTime.ToString("O"),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        IEnumerable<object> values => $"({string.Join(", ", values.Select(FormatValue))})",
        _ => value.ToString() ?? "null"
    };

    private static string Escape(string value) => value.Replace("'", "''");
}
