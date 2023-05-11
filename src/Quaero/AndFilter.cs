namespace Quaero;

public sealed class AndFilter : BinaryFilter
{
    public AndFilter(Filter left, Filter right) : base(left, right)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitAnd(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) => 
        visitor.VisitAnd(this);

    public override Filter Negate() => Or(Left.Negate(), Right.Negate());
    
    public override string ToString() => $"({Left} && {Right})";
}