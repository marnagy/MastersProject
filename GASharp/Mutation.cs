abstract class Mutation<T>
{
    protected double MutationProbability { get; private set; }
    public abstract T Mutate(T ind);
}