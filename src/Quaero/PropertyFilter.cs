namespace Quaero;

public abstract class PropertyFilter<T> : Filter
{
    protected PropertyFilter(string name, T value) => (Name, Value) = (name, value);

    public string Name { get; }

    public T Value { get; }
}
