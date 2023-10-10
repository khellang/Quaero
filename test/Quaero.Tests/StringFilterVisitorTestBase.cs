namespace Quaero.Tests;

[UsesVerify]
public abstract class StringFilterVisitorTestBase
{
    [Theory]
    [MemberData(nameof(PropertyValues))]
    public Task EqualFilter(string name, object value) => Verify(Convert(Equal(name, value))).UseParameters(name, value);

    public static IEnumerable<object?[]> PropertyValues
    {
        get
        {
            yield return new object[] { "id", new Guid("94957019-FC7D-417D-BD1B-147CC6113ED3") };
            yield return new object[] { "createdDate", new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(3)) };
            yield return new object?[] { "removedDate", null };
            yield return new object[] { "enabled", true };
            yield return new object[] { "enabled", false };
        }
    }

    [Fact]
    public Task NotEqualFilter() => Verify(Convert(NotEqual("disabled", true)));

    [Fact]
    public Task GreaterThanFilter() => Verify(Convert(GreaterThan("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc))));

    [Fact]
    public Task GreaterThanOrEqualFilter() => Verify(Convert(GreaterThanOrEqual("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc))));

    [Fact]
    public Task LessThanFilter() => Verify(Convert(LessThan("age", 69.5)));

    [Fact]
    public Task LessThanOrEqualFilter() => Verify(Convert(LessThanOrEqual("age", 69)));

    [Fact]
    public Task StartsWithFilter() => Verify(Convert(StartsWith("name", "John")));

    [Fact]
    public Task EndsWithFilter() => Verify(Convert(EndsWith("name", "Doe")));

    [Fact]
    public Task AndFilter() => Verify(Convert(StartsWith("name", "John").And(EndsWith("name", "Doe")).And(GreaterThanOrEqual("age", 42))));

    [Fact]
    public Task OrFilter() => Verify(Convert(StartsWith("name", "John").Or(EndsWith("name", "Doe")).Or(GreaterThanOrEqual("age", 42))));

    [Fact]
    public Task NotFilter() => Verify(Convert(Not(StartsWith("name", "John"))));

    [Fact]
    public Task InFilter() => Verify(Convert(In("department", "Retail", "Sales")));

    [Fact]
    public Task NullToString() => Verify(Convert(Equal("nullString", new ObjectWithNullReturningToString())));

    private string Convert(Filter filter) => ToString(filter.Optimize());

    protected abstract string ToString(Filter filter);

    // .NET's ToString method is documented with a
    // nullable return value. This object is used
    // to test the filter visitors' handling of this case.
    private class ObjectWithNullReturningToString
    {
        public override string? ToString() => null;
    }
}
