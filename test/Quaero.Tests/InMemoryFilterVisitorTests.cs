namespace Quaero.Tests;

public class InMemoryFilterVisitorTests : FilterVisitorTestBase
{
    protected override Task AssertFilter(Filter filter)
    {
        var predicate = filter.ToPredicate<TestObject>();

        var testObject = new TestObject
        {
            Id = new Guid("94957019-FC7D-417D-BD1B-147CC6113ED3"),
            Name = "John Doe",
            Age = 69,
            Department = "Retail",
            Date = new DateTime(2023, 01, 02),
            Enabled = true,
            NullString = new ObjectWithNullReturningToString(),
            RemovedDate = null,
            IsAdmin = false,
            CreatedDate = new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(3)),
        };

        Assert.True(predicate(testObject));

        return Task.CompletedTask;
    }

    private class TestObject
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }

        public required int Age { get; init; }

        public required string Department { get; init; }

        public required DateTime Date { get; init; }

        public required bool Enabled { get; init; }

        public required bool IsAdmin { get; init; }

        public required ObjectWithNullReturningToString NullString { get; init; }

        public required DateTime? RemovedDate { get; init; }

        public required DateTimeOffset CreatedDate { get; init; }
    }
}
