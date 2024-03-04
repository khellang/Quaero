namespace Quaero.Tests;

public abstract class FilterVisitorTestBase
{
    [Theory]
    [MemberData(nameof(PropertyValues))]
    public Task EqualFilter(string name, object value) => AssertFilter(Equal(name, value));

    public static IEnumerable<object?[]> PropertyValues
    {
        get
        {
            yield return new object[] { "id", new Guid("94957019-FC7D-417D-BD1B-147CC6113ED3") };
            yield return new object[] { "createdDate", new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(3)) };
            yield return new object?[] { "removedDate", null };
            yield return new object[] { "enabled", true };
            yield return new object[] { "isAdmin", false };
        }
    }

    [Fact]
    public Task NotEqualFilter() => AssertFilter(NotEqual("enabled", false));

    [Fact]
    public Task GreaterThanFilter() => AssertFilter(GreaterThan("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc)));

    [Fact]
    public Task GreaterThanOrEqualFilter() => AssertFilter(GreaterThanOrEqual("date", DateTime.SpecifyKind(new DateTime(2023, 01, 01), DateTimeKind.Utc)));

    [Fact]
    public Task LessThanFilter() => AssertFilter(LessThan("age", 70));

    [Fact]
    public Task LessThanOrEqualFilter() => AssertFilter(LessThanOrEqual("age", 69));

    [Fact]
    public Task StartsWithFilter() => AssertFilter(StartsWith("name", "John"));

    [Fact]
    public Task EndsWithFilter() => AssertFilter(EndsWith("name", "Doe"));

    [Fact]
    public Task ContainsFilter() => AssertFilter(Contains("name", "John"));

    [Fact]
    public Task AndFilter() => AssertFilter(StartsWith("name", "John").And(EndsWith("name", "Doe")).And(GreaterThanOrEqual("age", 42)));

    [Fact]
    public Task OrFilter() => AssertFilter(StartsWith("name", "John").Or(EndsWith("name", "Doe")).Or(GreaterThanOrEqual("age", 42)));

    [Fact]
    public Task NotFilter() => AssertFilter(Not(StartsWith("name", "Jack")));

    [Fact]
    public Task InFilter() => AssertFilter(In("department", "Retail", "Sales"));

    [Fact]
    public Task NullToString() => AssertFilter(NotEqual("nullString", new ObjectWithNullReturningToString()));

    protected abstract Task AssertFilter(Filter filter);

    // .NET's ToString method is documented with a
    // nullable return value. This object is used
    // to test the filter visitors' handling of this case.
    protected class ObjectWithNullReturningToString
    {
        public override string? ToString() => null;
    }
}
