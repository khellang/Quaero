using System.Globalization;

namespace Quaero;

public abstract class PropertyFilter<T> : Filter
{
    protected PropertyFilter(string name, T value) => (Name, Value) = (name, value);

    public string Name { get; }

    public T Value { get; }

    protected string FormatValue(T value) => value switch {
        null => "null",
        true => "true",
        false => "false",
        string str => $"'{Escape(str)}'",
        DateTime dateTime => dateTime.ToString("O"),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null"
    };
    
    private static string Escape(string value) => value.Replace("'", "''");
}