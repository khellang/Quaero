using Superpower;
using Superpower.Parsers;

namespace Quaero;

internal static class FilterTextParsers
{
    public static TextParser<Guid> Guid { get; } =
        from open in Character.EqualTo('\"')
        from content in Character.HexDigit.Or(Character.EqualTo('-')).Many()
        from close in Character.EqualTo('\"')
        select System.Guid.Parse(new string(content));
}
