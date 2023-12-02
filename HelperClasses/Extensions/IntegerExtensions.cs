public static class IntegerExtensions
{
    /// <summary>
    /// Copied from https://stackoverflow.com/a/11880606
    /// </summary>
    public static int Pow(this int baseNum, int power)
    => Enumerable
          .Repeat(baseNum, power)
          .Aggregate(1, (a, b) => a * b);
}