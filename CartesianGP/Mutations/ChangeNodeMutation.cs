public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private Random _rng = new Random();
    public double PercentageToChange { get; }
    public ChangeNodeMutation(double chromosomePercentageToChange, double probability): base(probability)
    {
        this.PercentageToChange = chromosomePercentageToChange;
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        lock(this)
        {
            rand_value = _rng.NextDouble();
        }

        // don't mutate
        if ( rand_value > this.MutationProbability )
            return ind.Clone();
        
        // mutate
        throw new NotImplementedException();
    }
}