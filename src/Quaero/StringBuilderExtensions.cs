using System.Text;

namespace Quaero;

/// <summary>
/// <see cref="StringFilterVisitor"/>-specific convenience methods for <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends to the specified <paramref name="builder"/>, using the specified <paramref name="visitor"/>
    /// to transform the specified <paramref name="filter"/>.
    /// </summary>
    /// <param name="builder">The builder to append to.</param>
    /// <param name="visitor">The visitor to use for transforming the filter.</param>
    /// <param name="filter">The filter to transform.</param>
    /// <returns>A modified <see cref="StringBuilder"/> instance.</returns>
    public static StringBuilder Append(this StringBuilder builder, StringFilterVisitor visitor, Filter filter) =>
        filter.Accept(visitor, builder);
}
