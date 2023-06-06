using Quaero.MicrosoftGraph;

// ReSharper disable once CheckNamespace
namespace Quaero;

public static class FilterExtensions
{
    public static string ToMicrosoftGraphQuery(this Filter filter) =>
        MicrosoftGraphFilterVisitor.Transform(filter);
}
