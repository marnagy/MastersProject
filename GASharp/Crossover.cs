public abstract class Crossover<T>
{
    public double CrossoverProbability { get; protected set; }
    public abstract Tuple<T, T> Cross(T ind1, T ind2);
}