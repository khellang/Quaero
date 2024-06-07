namespace Quaero.Tests;

public class FilterTests
{
    [Fact]
    public void AndWithNull_ShouldReturnOtherOperand()
    {
        var filter = Equal("test", 42);
        Assert.Same(filter, And(filter, null));
        Assert.Same(filter, And(null, filter));
    }

    [Fact]
    public void OrWithNull_ShouldReturnNull()
    {
        var filter = Equal("test", 42);
        Assert.Null(Or(null, null));
        Assert.Null(Or(null, filter));
        Assert.Null(Or(filter, null));
    }
}