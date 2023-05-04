public abstract class Selection<T>
{
    public abstract Tuple<T, T> ChooseParents(IList<T> population);
}