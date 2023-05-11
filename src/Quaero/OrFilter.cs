namespace Quaero;

public sealed class OrFilter : BinaryFilter
{
    public OrFilter(Filter left, Filter right) : base(left, right)
    {
    }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitOr(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitOr(this);

    public override Filter Negate() => And(Left.Negate(), Right.Negate());
    
    public override string ToString() => $"({Left} || {Right})";
}