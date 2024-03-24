public abstract class Mutation<T>
{
    public Mutation(double probability)
    {
        if (probability < 0d || probability > 1d)
            throw new ArgumentOutOfRangeException(
                $"Probability is expected from interval [0,1]."
                + "Received {probability}"
            );

        this.MutationProbability = probability;
    }
    /// <summary>
    /// Probability that the mutation will occur.
    /// </summary>
    public double MutationProbability { get; private set; }
    public abstract T Mutate(T ind, int genNum);
}