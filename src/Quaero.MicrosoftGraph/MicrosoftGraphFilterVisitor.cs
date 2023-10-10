using System.Globalization;
using System.Text;

namespace Quaero.MicrosoftGraph;

public sealed class MicrosoftGraphFilterVisitor : StringFilterVisitor
{
    private static readonly IFilterVisitor<string, StringBuilder> Instance = new MicrosoftGraphFilterVisitor();

    private MicrosoftGraphFilterVisitor() { }

    public static string Transform(Filter filter) => Instance.Visit(filter);

    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder) =>
        VisitBinary(filter, builder, "and");

    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder) =>
        VisitBinary(filter, builder, "or");

    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder) =>
        builder.Append("not(").Append(this, filter.Inner).Append(')');

    public override StringBuilder VisitEqual<TValue>(EqualFilter<TValue> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "eq");

    public override StringBuilder VisitNotEqual<TValue>(NotEqualFilter<TValue> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "ne");

    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        PrefixOperator(filter, builder, "startsWith");

    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) =>
        PrefixOperator(filter, builder, "endsWith");

    public override StringBuilder VisitGreaterThan(GreaterThanFilter filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "gt");

    public override StringBuilder VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "ge");

    public override StringBuilder VisitLessThan(LessThanFilter filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "lt");

    public override StringBuilder VisitLessThanOrEqual(LessThanOrEqualFilter filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "le");

    public override StringBuilder VisitIn<T>(InFilter<T> filter, StringBuilder builder) =>
        InfixOperator(filter, builder, "in");

    private StringBuilder VisitBinary(BinaryFilter filter, StringBuilder builder, string @operator) =>
        builder.Append('(').Append(this, filter.Left).Append(' ').Append(@operator).Append(' ').Append(this, filter.Right).Append(')');

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
