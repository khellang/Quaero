namespace Quaero;

public sealed class StartsWithFilter : PropertyFilter<string?>
{
    public StartsWithFilter(string name, string? value) : base(name, value) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitStartsWith(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) => 
        visitor.VisitStartsWith(this);

    public override string ToString() => $"startsWith({Name}, {Value})";
}