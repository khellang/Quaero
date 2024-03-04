namespace Quaero.Tests;

[UsesVerify]
public class FilterParserTests
{
    [Theory]
    [InlineData("userName eq \"khellang\"")]
    [InlineData("userName ne null")]
    [InlineData("age gt 18")]
    [InlineData("age le 21")]
    [InlineData("timestamp ge 1709586325")]
    [InlineData("timestamp lt 1709586325")]
    [InlineData("mail ew \"outlook.com\"")]
    [InlineData("domain sw \"example.com\"")]
    [InlineData("id eq 60 and id eq 1188")]
    [InlineData("id eq 60 or id eq 1188")]
    [InlineData("domain in (\"outlook.com\", \"example.com\")")]
    [InlineData("not(userName eq \"admin\")")]
    [InlineData("isEnabled eq true")]
    [InlineData("isEnabled ne false")]
    [InlineData("name co \"John\"")]
    public async Task Parse_Correct_Result(string value)
    {
        var settings = Verify(Parse(value).ToString());

        settings.UseParameters(value);

        await settings;
    }
}
