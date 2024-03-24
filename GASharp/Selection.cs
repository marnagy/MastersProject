public abstract class Selection<T>
{
    public abstract Tuple<T, T> ChooseParents(IReadOnlyList<T> population);
    /// <summary>
    /// Prevent having to recalculate probabilities by already providing them.
    /// </summary>
    public abstract Tuple<T, T> ChooseParents(
        IReadOnlyList<T> population,
        IReadOnlyList<double> probabilities
    );
}