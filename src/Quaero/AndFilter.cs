namespace Quaero;

public sealed class AndFilter : Filter
{
    public AndFilter(Filter left, Filter right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public Filter Left { get; }

    public Filter Right { get; }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitAnd(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) => 
        visitor.VisitAnd(this);

    public override Filter Negate() => Or(Left.Negate(), Right.Negate());
    
    public override string ToString() => $"({Left} && {Right})";
}