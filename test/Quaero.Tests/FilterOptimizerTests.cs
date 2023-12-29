namespace Quaero.Tests
{
    public class FilterOptimizerTests
    {
        [Fact]
        public void ShouldNormalizeDoubleNegation()
        {
            var filter = Not(Not(StartsWith("name", "John"))).Optimize();
            Assert.Equal(StartsWith("name", "John"), filter);
        }

        [Fact]
        public void ShouldNormalizeNotEqual()
        {
            var filter = Not(Equal("test", 42)).Optimize();
            Assert.Equal(NotEqual("test", 42), filter);
        }

        [Fact]
        public void ShouldNormalizeNegatedNotEqual()
        {
            var filter = Not(NotEqual("test", 42)).Optimize();
            Assert.Equal(Equal("test", 42), filter);
        }

        [Fact]
        public void ShouldNormalizeComparisonOperators()
        {
            var filter = Not(GreaterThan("test", 42)).Optimize();
            Assert.Equal(LessThanOrEqual("test", 42), filter);

            filter = Not(GreaterThanOrEqual("test", 42)).Optimize();
            Assert.Equal(LessThan("test", 42), filter);

            filter = Not(LessThan("test", 42)).Optimize();
            Assert.Equal(GreaterThanOrEqual("test", 42), filter);

            filter = Not(LessThanOrEqual("test", 42)).Optimize();
            Assert.Equal(GreaterThan("test", 42), filter);
        }
    }
}
