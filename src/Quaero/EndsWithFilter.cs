namespace Quaero;

public sealed class EndsWithFilter : PropertyFilter<string?>
{
    public EndsWithFilter(string name, string? value) : base(name, value) { }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitEndsWith(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) => 
        visitor.VisitEndsWith(this);

    public override string ToString() => $"endsWith({Name}, {Value})";
}