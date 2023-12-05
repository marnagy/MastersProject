public abstract class Mutation<T>
{
    protected Random _rng;
    public Mutation(double probability, int? seed = null)
    {
        if (probability < 0d || probability > 1d)
            throw new ArgumentOutOfRangeException($"Probability is expected from interval [0,1]. Received {probability}");

        this.MutationProbability = probability;
        this._rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }
    /// <summary>
    /// Probability that the mutation will occur.
    /// </summary>
    public double MutationProbability { get; private set; }
    public abstract T Mutate(T ind, int genNum);
}