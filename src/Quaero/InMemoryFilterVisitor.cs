using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Quaero;

/// <summary>
/// A visitor that transforms filter expressions into predicates that can be used for in-memory filtering.
/// This is typically used for connectors that don't support server-side filtering.
/// </summary>
public abstract class InMemoryFilterVisitor<T> : IFilterVisitor<Func<T, bool>>
{
    public static readonly InMemoryFilterVisitor<T> ReflectionBased = new ReflectionBasedInMemoryFilterVisitor();

    public Func<T, bool> Visit(Filter filter) => filter.Accept(this);

    public Func<T, bool> VisitAnd(AndFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return resource => left(resource) && right(resource);
    }

    public Func<T, bool> VisitOr(OrFilter filter)
    {
        var left = filter.Left.Accept(this);
        var right = filter.Right.Accept(this);
        return resource => left(resource) || right(resource);
    }

    public Func<T, bool> VisitNot(NotFilter filter)
    {
        var inner = filter.Inner.Accept(this);
        return resource => !inner(resource);
    }

    public Func<T, bool> VisitNotEqual<TValue>(NotEqualFilter<TValue> filter) =>
        VisitNot(new NotFilter(new EqualFilter<TValue>(filter.Name, filter.Value)));

    public Func<T, bool> VisitStartsWith(StartsWithFilter filter) =>
        resource => VisitStringFilter(resource, filter, (x, y) => x.StartsWith(y, StringComparison.Ordinal));

    public Func<T, bool> VisitEndsWith(EndsWithFilter filter) =>
        resource => VisitStringFilter(resource, filter, (x, y) => x.EndsWith(y, StringComparison.Ordinal));

    public Func<T, bool> VisitGreaterThan(GreaterThanFilter filter) =>
        resource => VisitTypedFilter(resource, filter, (x, y) => x.CompareTo(y) > 0);

    public Func<T, bool> VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter) =>
        resource => VisitTypedFilter(resource, filter, (x, y) => x.CompareTo(y) >= 0);

    public Func<T, bool> VisitLessThan(LessThanFilter filter) =>
        resource => VisitTypedFilter(resource, filter, (x, y) => x.CompareTo(y) < 0);

    public Func<T, bool> VisitLessThanOrEqual(LessThanOrEqualFilter filter) =>
        resource => VisitTypedFilter(resource, filter, (x, y) => x.CompareTo(y) <= 0);

    public Func<T, bool> VisitIn<TValue>(InFilter<TValue> filter) =>
        resource => TryGetPropertyValue<TValue>(resource, filter.Name, out var value) && filter.Value.Contains(value);

    public Func<T, bool> VisitEqual<TValue>(EqualFilter<TValue> filter) =>
        resource => VisitTypedFilter(resource, filter, (left, right) => EqualityComparer<TValue>.Default.Equals(left!, right!));

    protected abstract bool TryGetPropertyValue<TValue>(T resource, string name, [NotNullWhen(true)] out TValue? value);

    private bool VisitStringFilter(T resource, PropertyFilter<string?> filter, Func<string, string, bool> predicate) =>
        VisitTypedFilter(resource, filter, (x, y) => !string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y) && predicate(x!, y!));

    private bool VisitTypedFilter<TValue>(T resource, PropertyFilter<TValue> filter, Func<TValue, TValue, bool> predicate) =>
        TryGetPropertyValue<TValue>(resource, filter.Name, out var value) && predicate(value, filter.Value);

    private class ReflectionBasedInMemoryFilterVisitor : InMemoryFilterVisitor<T>
    {
        protected override bool TryGetPropertyValue<TValue>(T resource, string name, [NotNullWhen(true)] out TValue? value)
            where TValue : default
        {
            if (PropertyCache.Properties.TryGetValue(name, out var property) && property.CanRead)
            {
                value = (TValue)property.GetValue(resource);
                return true;
            }

            value = default;
            return false;
        }
    }

    private static class PropertyCache
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly Dictionary<string, PropertyInfo> Properties = GetProperties();

        private static Dictionary<string, PropertyInfo> GetProperties() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    }
}
