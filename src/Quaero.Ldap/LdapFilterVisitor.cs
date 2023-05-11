using System.Text;

namespace Quaero.Ldap;

public sealed class LdapFilterVisitor : StringFilterVisitor
{
    public static readonly IFilterVisitor<string, StringBuilder> Instance = new LdapFilterVisitor();

    private LdapFilterVisitor() { }

    public static string Transform(Filter filter) => Instance.Visit(filter);

    public override StringBuilder VisitAnd(AndFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(&");
        builder = filter.Left.Accept(this, builder);
        builder = filter.Right.Accept(this, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(|");
        builder = filter.Left.Accept(this, builder);
        builder = filter.Right.Accept(this, builder);
        return builder.Append(')');    
    }

    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(!");
        builder = filter.Inner.Accept(this, builder);
        return builder.Append(')');    
    }

    public override StringBuilder VisitEqual<T>(EqualFilter<T> filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder);

    public override StringBuilder VisitNotEqual<T>(NotEqualFilter<T> filter, StringBuilder builder) => 
        VisitNot(Filter.Not(Filter.Equal(filter.Name, filter.Value)), builder);

    public override StringBuilder VisitStartsWith(StartsWithFilter filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder, suffix: "*");

    public override StringBuilder VisitEndsWith(EndsWithFilter filter, StringBuilder builder) => 
        VisitPropertyFilter(filter, builder, prefix: "*");

    public override StringBuilder VisitGreaterThan(GreaterThanFilter filter, StringBuilder builder) =>
        VisitNot(Filter.Not(filter.Negate()), builder);

    public override StringBuilder VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, StringBuilder builder) => 
        VisitPropertyFilter(filter, builder, ">=");

    public override StringBuilder VisitLessThan(LessThanFilter filter, StringBuilder builder) => 
        VisitNot(Filter.Not(filter.Negate()), builder);

    public override StringBuilder VisitLessThanOrEqual(LessThanOrEqualFilter filter, StringBuilder builder) => 
        VisitPropertyFilter(filter, builder, "<=");

    private static string FormatValue<TValue>(TValue? value) => value?.ToString() ?? "null";

    private static StringBuilder VisitPropertyFilter<TValue>(PropertyFilter<TValue> filter, StringBuilder builder, string @operator = "=", string prefix = "", string suffix = "")
    {
        builder = builder.Append('(').Append(filter.Name).Append(@operator);
        if (!string.IsNullOrEmpty(prefix))
        {
            builder = builder.Append(prefix);
        }
        builder = builder.Append(FormatValue(filter.Value));
        if (!string.IsNullOrEmpty(suffix))
        {
            builder = builder.Append(suffix);
        }
        return builder.Append(')');
    }
}
