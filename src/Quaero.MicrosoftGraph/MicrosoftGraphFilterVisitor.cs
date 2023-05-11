using System.Globalization;
using System.Text;

namespace Quaero.MicrosoftGraph;

public sealed class MicrosoftGraphFilterVisitor : StringFilterVisitor
{
    public static readonly IFilterVisitor<string, StringBuilder> Instance = new MicrosoftGraphFilterVisitor();

    private MicrosoftGraphFilterVisitor() { }

    public static string Transform(Filter filter) => Instance.Visit(filter);

    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder)
    {
        builder = builder.Append('(');
        builder = filter.Left.Accept(this, builder);
        builder = builder.Append(" and ");
        builder = filter.Right.Accept(this, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder)
    {
        builder = builder.Append('(');
        builder = filter.Left.Accept(this, builder);
        builder = builder.Append(" or ");
        builder = filter.Right.Accept(this, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder)
    {
        builder = builder.Append("not(");
        builder = filter.Inner.Accept(this, builder);
        return builder.Append(')');
    }

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

    private static StringBuilder InfixOperator<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator) =>
        builder.Append(filter.Name).Append(' ').Append(@operator).Append(' ').Append(FormatValue(filter.Value));

    private static StringBuilder PrefixOperator<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator) =>
        builder.Append(@operator).Append('(').Append(filter.Name).Append(", ").Append(FormatValue(filter.Value)).Append(')');

    private static string FormatValue<TValue>(TValue? value) => value switch
    {
        null => "null",
        string @string => $"'{Escape(@string)}'",
        DateTime dateTime => dateTime.ToString("O"),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null"
    };

    private static string Escape(string value) => value.Replace("'", "''");
}
