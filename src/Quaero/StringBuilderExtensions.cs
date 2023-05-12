using System.Text;

namespace Quaero;

public static class StringBuilderExtensions
{
    public static StringBuilder Append(this StringBuilder builder, StringFilterVisitor visitor, Filter filter) => 
        filter.Accept(visitor, builder);
}
