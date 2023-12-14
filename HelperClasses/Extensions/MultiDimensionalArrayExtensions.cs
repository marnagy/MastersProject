public static class MultiDimensionalArrayExtensions
{
    public static IEnumerable<T> GetRow<T>(this T[,] arr, int rowIndex)
    {
        return Enumerable.Range(0, arr.GetLength(1))
               .Select(x => arr[rowIndex, x]);
    }
    public static IEnumerable<T> GetColumn<T>(this T[,] arr, int colIndex)
    {
        return Enumerable.Range(0, arr.GetLength(0))
               .Select(x => arr[x, colIndex]);
    }
}