abstract class Selection<T>
{
    public abstract Tuple<T, T> ChooseParents(IEnumerable<T> population);
}