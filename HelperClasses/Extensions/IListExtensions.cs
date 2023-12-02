using System.Text;

public static class IListExtensions
{
    public static string Stringify<T>(this IList<T> arr)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        sb.AppendJoin(", ", arr);
        sb.Append(']');
        return sb.ToString();
    }
}