using Superpower;
using Superpower.Parsers;

namespace Quaero;

internal static class FilterTextParsers
{
    public static TextParser<string> String { get; } =
        from open in Character.EqualTo('\"')
        from chars in Character.ExceptIn('\"', '\\')
            .Or(Character.EqualTo('\\')
                .IgnoreThen(
                    Character.EqualTo('\\')
                        .Or(Character.EqualTo('\"'))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Character.EqualTo('u').IgnoreThen(
                            Span.MatchedBy(Character.HexDigit.Repeat(4))
                                .Apply(Numerics.HexDigitsUInt32)
                                .Select(cc => (char)cc)))
                        .Named("escape sequence")))
            .Many()
        from close in Character.EqualTo('\"')
        select new string(chars);

    public static TextParser<Guid> Guid { get; } =
        from open in Character.EqualTo('\"')
        from content in Character.HexDigit.Or(Character.EqualTo('-')).Many()
        from close in Character.EqualTo('\"')
        select System.Guid.Parse(new string(content));

    public static TextParser<double> Number { get; } =
        from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
        from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
        from frac in Character.EqualTo('.')
            .IgnoreThen(Numerics.Natural)
            .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
            .OptionalOrDefault()
        from exp in Character.EqualToIgnoreCase('e')
            .IgnoreThen(Character.EqualTo('+').Value(1.0)
                .Or(Character.EqualTo('-').Value(-1.0))
                .OptionalOrDefault(1.0))
            .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
            .OptionalOrDefault()
        select (whole + frac) * sign * Math.Pow(10, exp);
}
