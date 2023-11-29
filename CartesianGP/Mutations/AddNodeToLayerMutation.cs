public class AddNodeToLayerMutation : Mutation<CartesianChromosome>
{
    private readonly Random _rng = new Random();
    public AddNodeToLayerMutation(double probability): base(probability) { }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        lock (_rng)
        {
            rand_value = _rng.NextDouble();
        }

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        // TODO: continue here
    }
}