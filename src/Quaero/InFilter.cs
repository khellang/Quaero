namespace Quaero;

public class InFilter<T> : PropertyFilter<ISet<T>>
{
    public InFilter(string name, IEnumerable<T> values) : base(name, new HashSet<T>(values)) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitIn(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitIn(this);

    public override string ToString() => $"{Name} in ({FormatValue(Value)})";
}
