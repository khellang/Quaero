namespace Quaero;

public abstract class BinaryFilter : Filter
{
    protected BinaryFilter(Filter left, Filter right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public Filter Left { get; }

    public Filter Right { get; }
}
