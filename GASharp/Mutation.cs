public abstract class Mutation<T>
{
    public double MutationProbability { get; protected set; }
    public abstract T Mutate(T ind, int genNum);
}