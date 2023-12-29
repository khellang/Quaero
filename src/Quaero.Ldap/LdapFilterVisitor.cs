using System.Globalization;
using System.Text;
using AntiLdapInjection;

namespace Quaero.Ldap;

/// <summary>
/// A visitor for converting filter expressions into LDAP query strings.
/// </summary>
public sealed class LdapFilterVisitor : StringFilterVisitor
{
    /// <summary>
    /// A singleton instance of the <see cref="LdapFilterVisitor"/>.
    /// </summary>
    public static readonly IFilterVisitor<string, StringBuilder> Instance = new LdapFilterVisitor();

    private LdapFilterVisitor() { }

    /// <inheritdoc />
    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(&");
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');
    }

    /// <inheritdoc />
    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(|");
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');
    }

    /// <inheritdoc />
    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder)
    {
        if (filter.Operand is EqualFilter<object> eq)
        {
            return VisitNotEqual(new NotEqualFilter<object>(eq.Name, eq.Value), builder);
        }

        return builder.Append("(!").Append(this, filter.Operand).Append(')');
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override StringBuilder VisitNotEqual<T>(NotEqualFilter<T> filter, StringBuilder builder)
    {
        if (filter.Value is null)
        {
            // This handles the following cases
            // - directReports ne null -> (directReports=*)
            // - not(directReports eq null) -> (directReports=*){
            return VisitFilter(filter.Name, string.Empty, builder, prefix: "*");
        }

        builder = builder.Append("(!");
        builder = VisitFilter(filter.Name, filter.Value, builder);
        return builder.Append(')');
    }

    /// <inheritdoc />
    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, suffix: "*");

    /// <inheritdoc />
    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, prefix: "*");

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThan<T>(GreaterThanFilter<T> filter, StringBuilder builder)
    {
        if (filter.Value is byte @byte)
        {
            return VisitFilter(filter.Name, @byte + 1, builder, ">=");
        }

        if (filter.Value is short @short)
        {
            return VisitFilter(filter.Name, @short + 1, builder, ">=");
        }

        if (filter.Value is int @int)
        {
            return VisitFilter(filter.Name, @int + 1, builder, ">=");
        }

        if (filter.Value is long @long)
        {
            return VisitFilter(filter.Name, @long + 1, builder, ">=");
        }

        if (filter.Value is double @double)
        {
            return VisitFilter(filter.Name, @double + 1, builder, ">=");
        }

        if (filter.Value is float @float)
        {
            return VisitFilter(filter.Name, @float + 1, builder, ">=");
        }

        return VisitNot(new NotFilter(filter.Negate()), builder);
    }

    /// <inheritdoc />
    public override StringBuilder VisitGreaterThanOrEqual<T>(GreaterThanOrEqualFilter<T> filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, ">=");

    /// <inheritdoc />
    public override StringBuilder VisitLessThan<T>(LessThanFilter<T> filter, StringBuilder builder)
    {
        if (filter.Value is byte @byte)
        {
            return VisitFilter(filter.Name, @byte - 1, builder, "<=");
        }

        if (filter.Value is short @short)
        {
            return VisitFilter(filter.Name, @short - 1, builder, "<=");
        }

        if (filter.Value is int @int)
        {
            return VisitFilter(filter.Name, @int - 1, builder, "<=");
        }

        if (filter.Value is long @long)
        {
            return VisitFilter(filter.Name, @long - 1, builder, "<=");
        }

        if (filter.Value is double @double)
        {
            return VisitFilter(filter.Name, @double - 1, builder, "<=");
        }

        if (filter.Value is float @float)
        {
            return VisitFilter(filter.Name, @float - 1, builder, "<=");
        }

        return VisitNot(new NotFilter(filter.Negate()), builder);
    }

    /// <inheritdoc />
    public override StringBuilder VisitLessThanOrEqual<T>(LessThanOrEqualFilter<T> filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, "<=");

    /// <inheritdoc />
    public override StringBuilder VisitIn<T>(InFilter<T> filter, StringBuilder builder)
    {
        builder = builder.Append("(|");
        builder = filter.Value.Aggregate(builder, (current, value) =>
            VisitEqual(new EqualFilter<T>(filter.Name, value), current));
        return builder.Append(')');
    }

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

    private static string? FormatValue<TValue>(TValue? value) => value switch
    {
        null => "NULL",
        true => "TRUE",
        false => "FALSE",
        Guid guid => Format(guid),
        string str => LdapEncoder.FilterEncode(str),
        DateTime dateTime => Format(dateTime.ToUniversalTime()),
        DateTimeOffset dateTimeOffset => Format(dateTimeOffset),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "NULL",
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
