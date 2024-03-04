namespace Quaero;

public sealed class PresenceFilter : PropertyFilter
{
    public PresenceFilter(string name) : base("pr", name)
    {
    }

    /// <inheritdoc />
    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitPresence(this, state);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) => 
        visitor.VisitPresence(this);

    /// <inheritdoc />
    public override string ToString() => $"{Name} {Operator}";
}