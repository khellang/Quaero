namespace Quaero;

public sealed class OrFilter : Filter
{
    public OrFilter(Filter left, Filter right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public Filter Left { get; }

    public Filter Right { get; }

    public override TState Accept<TResult, TState>(IFilterVisitor<TResult, TState> visitor, TState state) => 
        visitor.VisitOr(this, state);

    public override TResult Accept<TResult>(IFilterVisitor<TResult> visitor) =>
        visitor.VisitOr(this);

    public override Filter Negate() => And(Left.Negate(), Right.Negate());
    
    public override string ToString() => $"({Left} || {Right})";
}