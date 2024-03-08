using System.Diagnostics.CodeAnalysis;

namespace Quaero;

/// <summary>
/// A visitor that transforms filter expressions into predicates that can be used for in-memory filtering.
/// This is typically used for connectors that don't support server-side filtering.
/// </summary>
public abstract class InMemoryFilterVisitor<T> : IFilterVisitor<Func<T, bool>>
{
    /// <summary>
    /// A singleton instance of a reflection-based in-memory filter visitor.
    /// </summary>
    public static readonly InMemoryFilterVisitor<T> ReflectionBased = new ReflectionBasedInMemoryFilterVisitor();

    /// <inheritdoc />
    public Func<T, bool> Visit(Filter filter) => filter.Accept(this);

    /// <inheritdoc />
    public Func<T, bool> VisitAnd(AndFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return resource => left(resource) && right(resource);
    }

    /// <inheritdoc />
    public Func<T, bool> VisitOr(OrFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return resource => left(resource) || right(resource);
    }

    /// <inheritdoc />
    public Func<T, bool> VisitNot(NotFilter filter)
    {
        var inner = filter.Operand.Accept(this);
        return resource => !inner(resource);
    }

    /// <inheritdoc />
    public Func<T, bool> VisitNotEqual<TValue>(NotEqualFilter<TValue> filter) =>
        VisitNot(new NotFilter(new EqualFilter<TValue>(filter.Name, filter.Value)));

    /// <inheritdoc />
    public Func<T, bool> VisitStartsWith(StartsWithFilter filter) =>
        VisitStringFilter(filter, (x, y) => x.StartsWith(y, StringComparison.Ordinal));

    /// <inheritdoc />
    public Func<T, bool> VisitEndsWith(EndsWithFilter filter) =>
        VisitStringFilter(filter, (x, y) => x.EndsWith(y, StringComparison.Ordinal));

    /// <inheritdoc />
    public Func<T, bool> VisitContains(ContainsFilter filter) =>
        VisitStringFilter(filter, (x, y) => x.Contains(y));

    /// <inheritdoc />
    public Func<T, bool> VisitGreaterThan<TValue>(GreaterThanFilter<TValue> filter)
        where TValue : IComparable<TValue> =>
            VisitPropertyValueFilter(filter, (x, y) => x.CompareTo(y) > 0);

    /// <inheritdoc />
    public Func<T, bool> VisitGreaterThanOrEqual<TValue>(GreaterThanOrEqualFilter<TValue> filter)
        where TValue : IComparable<TValue> =>
            VisitPropertyValueFilter(filter, (x, y) => x.CompareTo(y) >= 0);

    /// <inheritdoc />
    public Func<T, bool> VisitLessThan<TValue>(LessThanFilter<TValue> filter)
        where TValue : IComparable<TValue> =>
            VisitPropertyValueFilter(filter, (x, y) => x.CompareTo(y) < 0);

    /// <inheritdoc />
    public Func<T, bool> VisitLessThanOrEqual<TValue>(LessThanOrEqualFilter<TValue> filter)
        where TValue : IComparable<TValue> =>
            VisitPropertyValueFilter(filter, (x, y) => x.CompareTo(y) <= 0);

    /// <inheritdoc />
    public Func<T, bool> VisitPresence(PresenceFilter filter)
    {
        var getValue = GetPropertyAccessor(filter.Name);
        return resource => getValue(resource) is not null;
    }

    /// <inheritdoc />
    public Func<T, bool> VisitIn<TValue>(InFilter<TValue> filter) =>
        VisitPropertyValueFilter<ISet<TValue>, TValue>(filter, (x, y) => y.Contains(x));

    /// <inheritdoc />
    public Func<T, bool> VisitEqual<TValue>(EqualFilter<TValue> filter) =>
        VisitPropertyValueFilter(filter, (left, right) => EqualityComparer<TValue>.Default.Equals(left!, right!));

    protected abstract bool TryGetPropertyAccessor(string name, [NotNullWhen(true)] out Func<T, object?>? accessor);

    private Func<T, bool> VisitStringFilter(PropertyValueFilter<string?> filter, Func<string, string, bool> predicate) =>
        VisitPropertyValueFilter(filter, (x, y) => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y) && predicate(x!, y!));

    private Func<T, bool> VisitPropertyValueFilter<TValue>(PropertyValueFilter<TValue> filter, Func<TValue, TValue, bool> predicate) =>
        VisitPropertyValueFilter<TValue, TValue>(filter, predicate);

    private Func<T, bool> VisitPropertyValueFilter<TFilter, TProperty>(PropertyValueFilter<TFilter> filter, Func<TProperty, TFilter, bool> predicate)
    {
        var getValue = GetPropertyAccessor(filter.Name);
        return resource => predicate((TProperty)getValue(resource)!, filter.Value);
    }

    private Func<T, object?> GetPropertyAccessor(string name)
    {
        if (!TryGetPropertyAccessor(name, out var getValue))
        {
            throw new MissingMemberException(typeof(T).FullName, name);
        }

        return getValue;
    }

    private sealed class ReflectionBasedInMemoryFilterVisitor : InMemoryFilterVisitor<T>
    {
        protected override bool TryGetPropertyAccessor(string name, [NotNullWhen(true)] out Func<T, object?>? accessor)
        {
            if (PropertyCache<T>.Properties.TryGetValue(name, out var property) && property.CanRead)
            {
                accessor = resource => property.GetValue(resource)!;
                return true;
            }

            accessor = default;
            return false;
        }
    }
}
