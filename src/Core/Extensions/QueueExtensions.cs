namespace AdventOfCode.Core.Extensions;
public static class QueueExtensions
{
    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            queue.Enqueue(item);
        }
    }

    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items, Func<T, bool> predicate)
    {
        foreach (var item in items.Where(predicate))
        {
            queue.Enqueue(item);
        }
    }
}
