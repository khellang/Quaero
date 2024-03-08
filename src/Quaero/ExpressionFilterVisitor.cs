using System.Linq.Expressions;
using System.Reflection;

namespace Quaero;

internal class ExpressionFilterVisitor<T> : IFilterVisitor<Expression>
{
    public static readonly ExpressionFilterVisitor<T> Instance = new();

    private ParameterExpression Parameter { get; } = Expression.Parameter(typeof(T), "x");

    public Expression<Func<T, bool>> ToLambda(Filter filter) =>
        Expression.Lambda<Func<T, bool>>(filter.Accept(this), Parameter);

    /// <inheritdoc />
    public Expression Visit(Filter filter) => filter.Accept(this);

    /// <inheritdoc />
    public Expression VisitAnd(AndFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return Expression.MakeBinary(ExpressionType.AndAlso, left, right);
    }

    /// <inheritdoc />
    public Expression VisitOr(OrFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return Expression.MakeBinary(ExpressionType.OrElse, left, right);
    }

    /// <inheritdoc />
    public Expression VisitNot(NotFilter filter)
    {
        var operand = filter.Operand.Accept(this);
        return Expression.Not(operand);
    }

    /// <inheritdoc />
    public Expression VisitEqual<TValue>(EqualFilter<TValue> filter) =>
        VisitPropertyValueFilter(filter, Expression.Equal);

    /// <inheritdoc />
    public Expression VisitNotEqual<TValue>(NotEqualFilter<TValue> filter) =>
        VisitPropertyValueFilter(filter, Expression.NotEqual);

    /// <inheritdoc />
    public Expression VisitStartsWith(StartsWithFilter filter) =>
        VisitStringFilter(filter, StringMethods.StartsWith);

    /// <inheritdoc />
    public Expression VisitEndsWith(EndsWithFilter filter) =>
        VisitStringFilter(filter, StringMethods.EndsWith);

    /// <inheritdoc />
    public Expression VisitContains(ContainsFilter filter) =>
        VisitStringFilter(filter, StringMethods.Contains);

    /// <inheritdoc />
    public Expression VisitGreaterThan<TValue>(GreaterThanFilter<TValue> filter) where TValue : IComparable<TValue> =>
        VisitPropertyValueFilter(filter, Expression.GreaterThan);

    /// <inheritdoc />
    public Expression VisitGreaterThanOrEqual<TValue>(GreaterThanOrEqualFilter<TValue> filter) where TValue : IComparable<TValue> =>
        VisitPropertyValueFilter(filter, Expression.GreaterThanOrEqual);

    /// <inheritdoc />
    public Expression VisitLessThan<TValue>(LessThanFilter<TValue> filter) where TValue : IComparable<TValue> =>
        VisitPropertyValueFilter(filter, Expression.LessThan);

    /// <inheritdoc />
    public Expression VisitLessThanOrEqual<TValue>(LessThanOrEqualFilter<TValue> filter) where TValue : IComparable<TValue> =>
        VisitPropertyValueFilter(filter, Expression.LessThanOrEqual);

    /// <inheritdoc />
    public Expression VisitPresence(PresenceFilter filter) =>
        VisitPropertyFilter(filter, member => Expression.NotEqual(member, Expression.Constant(null)));

    /// <inheritdoc />
    public Expression VisitIn<TValue>(InFilter<TValue> filter) =>
        VisitPropertyValueFilter(filter, (member, value) => Expression.Call(value, SetMethods<TValue>.Contains, member));

    private Expression VisitStringFilter(PropertyValueFilter<string?> filter, MethodInfo method) =>
        VisitPropertyValueFilter(filter, (member, value) => Expression.Call(member, method, value));

    private Expression VisitPropertyValueFilter<TValue>(PropertyValueFilter<TValue> filter, Func<Expression, Expression, Expression> getExpression)
    {
        var value = Expression.Constant(filter.Value, typeof(TValue));
        return VisitPropertyFilter(filter, member => getExpression(member, value));
    }

    private Expression VisitPropertyFilter(PropertyFilter filter, Func<Expression, Expression> getExpression) =>
        getExpression(GetMemberAccess(Parameter, filter.Name));

    private static MemberExpression GetMemberAccess(Expression expression, string name)
    {
        if (PropertyCache<T>.Properties.TryGetValue(name, out var propertyInfo))
        {
            return Expression.MakeMemberAccess(expression, propertyInfo);
        }

        throw new MissingMemberException(typeof(T).FullName, name);
    }
}
