public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private Random _rng = new Random();
    public double PercentageToChange { get; }
    private readonly Dictionary<int, List<CartesianNode>> NodeCatalogue;
    public ChangeNodeMutation(double chromosomePercentageToChange, double probability, Dictionary<int, List<CartesianNode>> nodeCatalogue): base(probability)
    {
        this.PercentageToChange = chromosomePercentageToChange;
        this.NodeCatalogue = nodeCatalogue;
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        lock(_rng)
        {
            rand_value = _rng.NextDouble();
        }

        // don't mutate
        if ( rand_value > this.MutationProbability )
            return ind.Clone();
        
        // mutate
        List<List<double>> node_probabilities;
        lock(_rng)
        {
            // TODO: continue here with NodeCatalogue
            node_probabilities = ind.GetLayerSizes()
                .Select(layerSize => Enumerable.Range(0, layerSize)
                    .Select(_ => _rng.)
                )
        }
    }
}