namespace Quaero;

public sealed class NotFilter : Filter
{
    public NotFilter(Filter inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public Filter Inner { get; }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitNot(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitNot(this);

    public override Filter Negate() => Inner;

    public override string ToString() => $"!({Inner})";
}