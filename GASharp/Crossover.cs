abstract class Crossover<T>
{
    protected double CrossoverProbability { get; private set; }
    public abstract Tuple<T, T> Cross(T ind1, T ind2);
}