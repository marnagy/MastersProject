public static class MultiDimensionalArrayExtensions
{
    public static int GetRowsAmount<T>(this T[,] arr)
    => arr.GetLength(0);
    public static int GetColumnsAmount<T>(this T[,] arr)
    => arr.GetLength(1);
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
    public static IEnumerable<IEnumerable<T>> IterateRows<T>(this T[,] arr)
    => Enumerable.Range(0, arr.GetRowsAmount())
        .Select(rowIndex => arr.GetRow(rowIndex));
}