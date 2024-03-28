public class ChangeParentsMutation : Mutation<CartesianChromosome>
{
    public double PercentageToChange { get; }
    public ChangeParentsMutation(double chromosomePercentageToChange, double probability) : base(probability)
    {
        this.PercentageToChange = chromosomePercentageToChange;
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
                .Select(_ => Random.Shared.NextDouble() < this.PercentageToChange)
                .ToArray()
            )
            .ToArray();

        var layers = ind.DeepCopyLayers();
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            for (int j = 0; j < layers[layerIndex].Count; j++)
            {
                if (shouldNodeMutate[layerIndex][j])
                {
                    // !: FIX THIS
                    // choose new parent nodes
                    // choose layer and index within uniformly
                    ParentIndices[] newParents;
                    // newParents = Enumerable.Range(0, CartesianNode.ParentsAmount)
                    //     .Select(_ => Random.Shared.Next(i + 1))
                    //     .Select(layerIndex => new ParentIndices
                    //         {
                    //             LayerIndex=layerIndex,
                    //             Index=Random.Shared.Next(ind[layerIndex].Count)
                    //         }
                    //     )
                    //     .ToArray();
                    newParents = CartesianChromosome.ChooseParents(
                        inputsAmount: ind.InputsAmount,
                        layers,
                        internalLayerIndex: layerIndex
                    );

                    // System.Console.Error.WriteLine($"PreviousParents: {layers[i][j].Parents.Stringify()}");
                    layers[layerIndex][j] =  layers[layerIndex][j].Clone(newParents);
                    // System.Console.Error.WriteLine($"Parents after mutation: {layers[i][j].Parents.Stringify()}");
                }
            }
        }

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        // System.Console.Error.WriteLine($"ChangeNodeCreated valid chromosome? {CartesianChromosome.IsValid(newChromosome)}");

        if ( !CartesianChromosome.IsValid(newChromosome) )
            throw new Exception($"Created invalid chromosome in {this.GetType()}!");

        return newChromosome;
    }
}