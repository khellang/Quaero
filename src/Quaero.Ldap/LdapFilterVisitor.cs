using System.Globalization;
using System.Text;
using AntiLdapInjection;

namespace Quaero.Ldap;

public sealed class LdapFilterVisitor : StringFilterVisitor
{
    private static readonly IFilterVisitor<string, StringBuilder> Instance = new LdapFilterVisitor();

    private LdapFilterVisitor() { }

    public static string Transform(Filter filter) => Instance.Visit(filter);

    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(&");
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(|");
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder)
    {
        if (filter.Inner is EqualFilter<object> { Value: null } eq)
        {
            // This handles the following cases
            // - directReports ne null -> (directReports=*)
            // - not(directReports eq null) -> (directReports=*)
            return VisitFilter(eq.Name, string.Empty, builder, prefix: "*");
        }

        return builder.Append("(!").Append(this, filter.Inner).Append(')');
    }

    public override StringBuilder VisitEqual<T>(EqualFilter<T> filter, StringBuilder builder)
    {
        if (filter.Value is null)
        {
            // This handles the following case:
            // - manager eq null -> (!(manager=*))
            builder = builder.Append("(!");
            builder = VisitFilter(filter.Name, string.Empty, builder, prefix: "*");
            return builder.Append(')');
        }

        return VisitPropertyFilter(filter, builder);
    }

    public override StringBuilder VisitNotEqual<T>(NotEqualFilter<T> filter, StringBuilder builder) =>
        VisitNot(new NotFilter(Filter.Equal(filter.Name, filter.Value)), builder);

    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, suffix: "*");

    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, prefix: "*");

    public override StringBuilder VisitGreaterThan(GreaterThanFilter filter, StringBuilder builder) =>
        VisitNot(new NotFilter(filter.Negate()), builder);

    public override StringBuilder VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, ">=");

    public override StringBuilder VisitLessThan(LessThanFilter filter, StringBuilder builder) =>
        VisitNot(new NotFilter(filter.Negate()), builder);

    public override StringBuilder VisitLessThanOrEqual(LessThanOrEqualFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, "<=");

    private static StringBuilder VisitPropertyFilter<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator = "=", string prefix = "", string suffix = "") =>
        VisitFilter(filter.Name, filter.Value, builder, @operator, prefix, suffix);

    private static StringBuilder VisitFilter<TValue>(string name, TValue value, StringBuilder builder, string @operator = "=", string prefix = "", string suffix = "")
    {
        builder = builder.Append('(').Append(name).Append(@operator);
        if (!string.IsNullOrEmpty(prefix))
        {
            builder = builder.Append(prefix);
        }
        builder = builder.Append(FormatValue(value));
        if (!string.IsNullOrEmpty(suffix))
        {
            builder = builder.Append(suffix);
        }
        return builder.Append(')');
    }

    private static string FormatValue<TValue>(TValue? value) => value switch
    {
        null => "NULL",
        true => "TRUE",
        false => "FALSE",
        Guid guid => Format(guid),
        string str => LdapEncoder.FilterEncode(str),
        DateTime dateTime => Format(dateTime.ToUniversalTime()),
        DateTimeOffset dateTimeOffset => Format(dateTimeOffset),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value?.ToString() ?? "NULL"
    };

    private static string Format(DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToFileTime().ToString(CultureInfo.InvariantCulture);

    private static string Format(Guid guid) => Format(guid.ToByteArray());

    private static string Format(byte[] bytes)
    {
        var builder = new StringBuilder(capacity: bytes.Length * 3);

        for (var index = 0; index < bytes.Length; index++)
        {
            builder.Append('\\').Append(bytes[index].ToString("X2"));
        }

        return builder.ToString();
    }

    private static StringBuilder VisitBinaryChain<TFilter>(LdapFilterVisitor visitor, TFilter filter, StringBuilder builder)
        where TFilter : BinaryFilter
    {
        builder = VisitBranch(visitor, filter.Left, builder);
        return VisitBranch(visitor, filter.Right, builder);

        static StringBuilder VisitBranch(LdapFilterVisitor visitor, Filter filter, StringBuilder builder)
        {
            if (filter is TFilter branch)
            {
                return VisitBinaryChain(visitor, branch, builder);
            }

            return filter.Accept(visitor, builder);
        }
    }
}
