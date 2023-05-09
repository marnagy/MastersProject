public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private Random _rng = new Random();
    public double PercentageToChange { get; }
    private readonly IReadOnlyDictionary<int, IList<CartesianNode>> NodeCatalogue;
    private readonly IList<CartesianNode> Nodes;
    public ChangeNodeMutation(double chromosomePercentageToChange, double probability, Dictionary<int, IList<CartesianNode>> nodeCatalogue): base(probability)
    {
        this.PercentageToChange = chromosomePercentageToChange;
        this.NodeCatalogue = nodeCatalogue;
        this.Nodes = nodeCatalogue.Keys
            .SelectMany(arity => nodeCatalogue[arity])
            .ToArray();
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
        IList<IList<double>> nodeProbabilities;
        lock(_rng)
        {
            nodeProbabilities = ind.GetLayerSizes()
                .Select(layerSize => Enumerable.Range(0, layerSize)
                    .Select(_ => _rng.NextDouble())
                    .ToArray()
                )
                .ToArray();
        }

        // TODO: replace node with probability < this.PercentageToChange
        var layers = ind.DeepCopyLayers();
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                if ( nodeProbabilities[i][j] < this.PercentageToChange )
                {
                    // choose new random node, preserve parents
                    layers[i][j] = _rng.Choose<CartesianNode>(this.Nodes).Clone(layers[i][j].Parents);
                }
            }
        }

        return new CartesianChromosome(
            ind.InputsAmount,
            layers
        );
    }
}