namespace Quaero;

/// <summary>
/// A stateful visitor to transform filter expressions into target system-specific queries.
/// </summary>
/// <typeparam name="TResult">The type of result the visitor produces.</typeparam>
/// <typeparam name="TState">The type of state the visitor operates on.</typeparam>
public interface IFilterVisitor<out TResult, TState>
{
    TResult Visit(Filter filter);

    TState VisitAnd(AndFilter filter, TState state);

    TState VisitOr(OrFilter filter, TState state);

    TState VisitNot(NotFilter filter, TState state);

    TState VisitEqual<T>(EqualFilter<T> filter, TState state);

    TState VisitNotEqual<T>(NotEqualFilter<T> filter, TState state);

    TState VisitStartsWith(StartsWithFilter filter, TState state);

    TState VisitEndsWith(EndsWithFilter filter, TState state);

    TState VisitGreaterThan(GreaterThanFilter filter, TState state);

    TState VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, TState state);

    TState VisitLessThan(LessThanFilter filter, TState state);

    TState VisitLessThanOrEqual(LessThanOrEqualFilter filter, TState state);

    TState VisitIn<T>(InFilter<T> filter, TState state);
}

/// <summary>
/// A visitor to transform filter expressions into target system-specific queries.
/// </summary>
/// <typeparam name="TResult">The type of result the visitor produces.</typeparam>
public interface IFilterVisitor<out TResult>
{
    TResult Visit(Filter filter);

    TResult VisitAnd(AndFilter filter);

    TResult VisitOr(OrFilter filter);

    TResult VisitNot(NotFilter filter);

    TResult VisitEqual<T>(EqualFilter<T> filter);

    TResult VisitNotEqual<T>(NotEqualFilter<T> filter);

    TResult VisitStartsWith(StartsWithFilter filter);

    TResult VisitEndsWith(EndsWithFilter filter);

    TResult VisitGreaterThan(GreaterThanFilter filter);

    TResult VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter);

    TResult VisitLessThan(LessThanFilter filter);

    TResult VisitLessThanOrEqual(LessThanOrEqualFilter filter);

    TResult VisitIn<T>(InFilter<T> filter);
}
