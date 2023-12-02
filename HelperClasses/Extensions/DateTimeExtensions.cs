public static class DateTimeExtensions
{
    public static int GetTimestamp(this DateTime value)
    {
        return (int)long.Parse(value.ToString("yyyyMMddHHmmssffff"));
    }
}