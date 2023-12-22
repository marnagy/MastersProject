public abstract class Selection<T>
{
    protected readonly Random _rng;
    public abstract Tuple<T, T> ChooseParents(IReadOnlyList<T> population);
    protected Selection(int? seed)
    {
        this._rng = seed.HasValue
            ? new Random(seed.Value)
            : new Random();
    }
}