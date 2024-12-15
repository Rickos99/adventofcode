namespace AdventOfCode.Core.Extensions;
public static class EnumerableExtensions
{
    public static IEnumerable<TElement> RemoveElementAt<TElement>(this IEnumerable<TElement> source, int index)
        => source.Take(index).Concat(source.Skip(index + 1));

    public static int IndexOfFirst<TElement>(this IEnumerable<TElement> source, Func<TElement, bool> predicate)
    {
        var i = 0;
        foreach (var element in source)
        {
            if (predicate(element)) return i;
            else i++;
        }
        throw new InvalidOperationException("Provided collection does not contain any element that meet the predicate");
    }
}
