using System.Text;

public static class IEnumerableExtensions
{
    public static string Stringify<T>(this IEnumerable<T> arr)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        sb.AppendJoin(", ", arr);
        sb.Append(']');
        return sb.ToString();
    }
    public static void ForEach<T>(this IEnumerable<T> arr, Action<T> action)
    {
        foreach (var item in arr)
        {
            action(item);
        }
    }
}