public class ChangeParentsMutation : Mutation<CartesianChromosome>
{
    public double PercentageToChange { get; }
    private readonly IReadOnlyDictionary<int, IList<CartesianNode>> NodeCatalogue;
    private readonly IList<CartesianNode> Nodes;
    public ChangeParentsMutation(double chromosomePercentageToChange, double probability,
            Dictionary<int, IList<CartesianNode>> nodeCatalogue) : base(probability)
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
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        // mutate
        IList<IList<bool>> shouldNodeMutate;
        shouldNodeMutate = ind.GetLayerSizes()
            .Select(layerSize => Enumerable.Range(0, layerSize)
                .Select(_ => Random.Shared.NextDouble())
                .Select(prob => prob < this.PercentageToChange)
                .ToArray()
            )
            .ToArray();

        var layers = ind.DeepCopyLayers();
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                if (shouldNodeMutate[i][j])
                {
                    // choose new parent nodes
                    // choose layer and index within uniformly
                    ParentIndices[] newParents;
                    newParents = Enumerable.Range(0, CartesianNode.ParentsAmount)
                        .Select(_ => Random.Shared.Next(j + 1))
                        .Select(layerIndex => new ParentIndices
                            {
                                LayerIndex=layerIndex,
                                Index=Random.Shared.Next(ind[layerIndex].Count)
                            }
                        )
                        .ToArray();

                    System.Console.Error.WriteLine($"PreviousParents: {layers[i][j].Parents.Stringify()}");
                    layers[i][j] =  layers[i][j].Clone(newParents);
                    System.Console.Error.WriteLine($"Parents after mutation: {layers[i][j].Parents.Stringify()}");
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