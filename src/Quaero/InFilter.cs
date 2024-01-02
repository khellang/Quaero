namespace Quaero;

public class InFilter<T> : PropertyFilter<ISet<T>>
{
    public InFilter(string name, HashSet<T> values) : base("in", name, values) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) =>
        visitor.VisitIn(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitIn(this);
}
