namespace Quaero;

/// <summary>
/// A stateful visitor to transform filter expressions into target system-specific queries.
/// </summary>
/// <typeparam name="TResult">The type of result the visitor produces.</typeparam>
/// <typeparam name="TState">The type of state the visitor operates on.</typeparam>
public interface IFilterVisitor<out TResult, TState>
{
    /// <summary>
    /// The base method for visiting a filter.
    /// </summary>
    /// <param name="filter">The filter to visit.</param>
    /// <returns>The result of the visit.</returns>
    TResult Visit(Filter filter);

    /// <summary>
    /// Visits an <see cref="AndFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitAnd(AndFilter filter, TState state);

    /// <summary>
    /// Visits an <see cref="OrFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitOr(OrFilter filter, TState state);

    /// <summary>
    /// Visits a <see cref="NotFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitNot(NotFilter filter, TState state);

    /// <summary>
    /// Visits an <see cref="EqualFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <typeparam name="T">The type fo the value being checked for equality.</typeparam>
    /// <returns>The result of the visit.</returns>
    TState VisitEqual<T>(EqualFilter<T> filter, TState state);

    /// <summary>
    /// Visits a <see cref="NotEqualFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <typeparam name="T">The type fo the value being checked for inequality.</typeparam>
    /// <returns>The result of the visit.</returns>
    TState VisitNotEqual<T>(NotEqualFilter<T> filter, TState state);

    /// <summary>
    /// Visits a <see cref="StartsWithFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitStartsWith(StartsWithFilter filter, TState state);

    /// <summary>
    /// Visits an <see cref="EndsWithFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitEndsWith(EndsWithFilter filter, TState state);

    /// <summary>
    /// Visits a <see cref="GreaterThanFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitGreaterThan(GreaterThanFilter filter, TState state);

    /// <summary>
    /// Visits a <see cref="GreaterThanOrEqualFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter, TState state);

    /// <summary>
    /// Visits a <see cref="LessThanFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitLessThan(LessThanFilter filter, TState state);

    /// <summary>
    /// Visits a <see cref="LessThanOrEqualFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <returns>The result of the visit.</returns>
    TState VisitLessThanOrEqual(LessThanOrEqualFilter filter, TState state);

    /// <summary>
    /// Visits an <see cref="InFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <param name="state">The current state.</param>
    /// <typeparam name="T">The type of values being checked against.</typeparam>
    /// <returns>The result of the visit.</returns>
    TState VisitIn<T>(InFilter<T> filter, TState state);
}

/// <summary>
/// A visitor to transform filter expressions into target system-specific queries.
/// </summary>
/// <typeparam name="TResult">The type of result the visitor produces.</typeparam>
public interface IFilterVisitor<out TResult>
{
    /// <summary>
    /// The base method for visiting a filter.
    /// </summary>
    /// <param name="filter">The filter to visit.</param>
    /// <returns>The result of the visit.</returns>
    TResult Visit(Filter filter);

    /// <summary>
    /// Visits an <see cref="AndFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitAnd(AndFilter filter);

    /// <summary>
    /// Visits an <see cref="OrFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitOr(OrFilter filter);

    /// <summary>
    /// Visits a <see cref="NotFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitNot(NotFilter filter);

    /// <summary>
    /// Visits an <see cref="EqualFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <typeparam name="T">The type fo the value being checked for equality.</typeparam>
    /// <returns>The result of the visit.</returns>
    TResult VisitEqual<T>(EqualFilter<T> filter);

    /// <summary>
    /// Visits a <see cref="NotEqualFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <typeparam name="T">The type fo the value being checked for inequality.</typeparam>
    /// <returns>The result of the visit.</returns>
    TResult VisitNotEqual<T>(NotEqualFilter<T> filter);

    /// <summary>
    /// Visits a <see cref="StartsWithFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitStartsWith(StartsWithFilter filter);

    /// <summary>
    /// Visits an <see cref="EndsWithFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitEndsWith(EndsWithFilter filter);

    /// <summary>
    /// Visits a <see cref="GreaterThanFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitGreaterThan(GreaterThanFilter filter);

    /// <summary>
    /// Visits a <see cref="GreaterThanOrEqualFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitGreaterThanOrEqual(GreaterThanOrEqualFilter filter);

    /// <summary>
    /// Visits a <see cref="LessThanFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitLessThan(LessThanFilter filter);

    /// <summary>
    /// Visits a <see cref="LessThanOrEqualFilter"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <returns>The result of the visit.</returns>
    TResult VisitLessThanOrEqual(LessThanOrEqualFilter filter);

    /// <summary>
    /// Visits an <see cref="InFilter{T}"/>.
    /// </summary>
    /// <param name="filter">The filter being visited.</param>
    /// <typeparam name="T">The type of values being checked against.</typeparam>
    /// <returns>The result of the visit.</returns>
    TResult VisitIn<T>(InFilter<T> filter);
}
