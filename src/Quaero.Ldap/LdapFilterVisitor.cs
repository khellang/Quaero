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
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');
    }

    public override StringBuilder VisitOr(OrFilter filter, StringBuilder builder)
    {
        builder = builder.Append("(|");
        builder = VisitBinaryChain(this, filter, builder);
        return builder.Append(')');    
    }

    public override StringBuilder VisitNot(NotFilter filter, StringBuilder builder) => 
        builder.Append("(!").Append(this, filter.Inner).Append(')');

    public override StringBuilder VisitEqual<T>(EqualFilter<T> filter, StringBuilder builder) =>
        VisitPropertyFilter(filter, builder);

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
