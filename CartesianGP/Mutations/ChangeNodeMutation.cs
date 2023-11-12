public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private Random _rng = new Random();
    public double PercentageToChange { get; }
    private readonly IReadOnlyDictionary<int, IList<CartesianNode>> NodeCatalogue;
    private readonly IList<CartesianNode> Nodes;
    public ChangeNodeMutation(double chromosomePercentageToChange, double probability, Dictionary<int, IList<CartesianNode>> nodeCatalogue) : base(probability)
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
        lock (_rng)
        {
            rand_value = _rng.NextDouble();
        }

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        // mutate
        IList<IList<double>> nodeProbabilities;
        lock (_rng)
        {
            nodeProbabilities = ind.GetLayerSizes()
                .Select(layerSize => Enumerable.Range(0, layerSize)
                    .Select(_ => _rng.NextDouble())
                    .ToArray()
                )
                .ToArray();
        }

        var layers = ind.DeepCopyLayers();
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                if (nodeProbabilities[i][j] < this.PercentageToChange)
                {
                    // choose new random node, preserve parents
                    System.Console.Error.WriteLine($"PreviousParents: {layers[i][j].Parents}");
                    layers[i][j] = _rng.Choose(this.Nodes).Clone(layers[i][j].Parents);
                    System.Console.Error.WriteLine($"Parents after mutation: {layers[i][j].Parents}");
                }
            }
        }

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        System.Console.Error.WriteLine($"ChangeNodeCreated valid chromosome? {CartesianChromosome.IsValid(newChromosome)}");

        // if ( !CartesianChromosome.IsValid(newChromosome) )
        //     throw new Exception($"Created invalid chromosome in {this.GetType()}!");

        return newChromosome;
    }
}