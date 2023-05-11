static class DateTimeExtensions
{
    public static int GetTimestamp(this DateTime value)
    {
        return int.Parse( value.ToString("yyyyMMddHHmmssffff") );
    }
}