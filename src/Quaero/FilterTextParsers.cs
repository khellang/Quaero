using Superpower;
using Superpower.Parsers;

namespace Quaero;

internal static class FilterTextParsers
{
    public static TextParser<Guid> Guid { get; } =
        from open in Character.EqualTo('\"')
        from content in Character.HexDigit.Or(Character.EqualTo('-')).Many()
        from close in Character.EqualTo('\"')
#if NET8_0_OR_GREATER
        select System.Guid.Parse(content);
#else
        select System.Guid.Parse(new string(content));
#endif
}
