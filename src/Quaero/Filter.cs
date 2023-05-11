namespace Quaero;

public abstract class Filter
{
    public static EqualFilter<T> Equal<T>(string name, T? value) => new(name, value);

    public static NotEqualFilter<T> NotEqual<T>(string name, T? value) => new(name, value);
    
    public static StartsWithFilter StartsWith(string name, string? value) => new(name, value);

    public static EndsWithFilter EndsWith(string name, string? value) => new(name, value);

    public static GreaterThanFilter GreaterThan(string name, IComparable value) => new(name, value);

    public static GreaterThanOrEqualFilter GreaterThanOrEqual(string name, IComparable value) => new(name, value);
    
    public static LessThanFilter LessThan(string name, IComparable value) => new(name, value);

    public static LessThanOrEqualFilter LessThanOrEqual(string name, IComparable value) => new(name, value);

    public static NotFilter Not(Filter filter) => new(filter);

    public static AndFilter And(Filter left, Filter right) => new(left, right);

    public static OrFilter Or(Filter left, Filter right) => new(left, right);

    public AndFilter And(Filter other) => And(this, other);

    public OrFilter Or(Filter other) => Or(this, other);

    public virtual Filter Negate() => Not(this);
    
    public abstract TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state);

    public abstract TResult Accept<TResult>(IFilterVisitor<TResult> visitor);

    public abstract override string ToString();
}
