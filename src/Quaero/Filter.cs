namespace Quaero;

public abstract class Filter
{
    public static Filter Equal<T>(string name, T? value) => new EqualFilter<T>(name, value);

    public static Filter NotEqual<T>(string name, T? value) => new NotEqualFilter<T>(name, value);
    
    public static Filter StartsWith(string name, string? value) => new StartsWithFilter(name, value);

    public static Filter EndsWith(string name, string? value) => new EndsWithFilter(name, value);

    public static Filter GreaterThan(string name, IComparable value) => new GreaterThanFilter(name, value);

    public static Filter GreaterThanOrEqual(string name, IComparable value) => new GreaterThanOrEqualFilter(name, value);
    
    public static Filter LessThan(string name, IComparable value) => new LessThanFilter(name, value);

    public static Filter LessThanOrEqual(string name, IComparable value) => new LessThanOrEqualFilter(name, value);

    public static Filter Not(Filter filter) => new NotFilter(filter);

    public static Filter And(Filter left, Filter right) => new AndFilter(left, right);

    public static Filter Or(Filter left, Filter right) => new OrFilter(left, right);

    public Filter And(Filter other) => And(this, other);

    public Filter Or(Filter other) => Or(this, other);

    public virtual Filter Negate() => Not(this);
    
    public abstract TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state);

    public abstract TResult Accept<TResult>(IFilterVisitor<TResult> visitor);

    public abstract override string ToString();
}
