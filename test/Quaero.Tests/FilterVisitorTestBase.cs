namespace Quaero.Tests;

public abstract class FilterVisitorTestBase
{
    [Theory]
    [MemberData(nameof(PropertyValues))]
    public Task EqualFilter(string name, object value) => Assert(Equal(name, value));

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
    public Task NotEqualFilter() => Assert(NotEqual("disabled", true));

    [Fact]
    public Task GreaterThanFilter() => Assert(GreaterThan("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc)));

    [Fact]
    public Task GreaterThanOrEqualFilter() => Assert(GreaterThanOrEqual("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc)));

    [Fact]
    public Task LessThanFilter() => Assert(LessThan("age", 69.5));

    [Fact]
    public Task LessThanOrEqualFilter() => Assert(LessThanOrEqual("age", 69));

    [Fact]
    public Task StartsWithFilter() => Assert(StartsWith("name", "John"));

    [Fact]
    public Task EndsWithFilter() => Assert(EndsWith("name", "Doe"));

    [Fact]
    public Task AndFilter() => Assert(StartsWith("name", "John").And(EndsWith("name", "Doe")).And(GreaterThanOrEqual("age", 42)));

    [Fact]
    public Task OrFilter() => Assert(StartsWith("name", "John").Or(EndsWith("name", "Doe")).Or(GreaterThanOrEqual("age", 42)));

    [Fact]
    public Task NotFilter() => Assert(Not(StartsWith("name", "John")));

    [Fact]
    public Task InFilter() => Assert(In("department", "Retail", "Sales"));

    [Fact]
    public Task NullToString() => Assert(Equal("nullString", new ObjectWithNullReturningToString()));

    protected abstract Task Assert(Filter filter);

    // .NET's ToString method is documented with a
    // nullable return value. This object is used
    // to test the filter visitors' handling of this case.
    private class ObjectWithNullReturningToString
    {
        public override string? ToString() => null;
    }
}
