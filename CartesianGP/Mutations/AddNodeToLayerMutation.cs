public class AddNodeToLayerMutation : Mutation<CartesianChromosome>
{
    private readonly IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> NodeCatalogue;
    private readonly IReadOnlyList<CartesianNode> Nodes;
    public AddNodeToLayerMutation(double probability,
        IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> nodeCatalogue): base(probability)
    {
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

        var layers = ind.DeepCopyLayers();
        
        int indexOfLayerAddNodeTo;
        // don't change output layer
        indexOfLayerAddNodeTo = Random.Shared.Next(layers.Count - 1);

        var parents = Enumerable.Range(0, CartesianNode.ParentsAmount)
            .Select(_ => { 
                // include input layer
                var parentLayerIndex = Random.Shared.Next(indexOfLayerAddNodeTo + 1);
                return new ParentIndices(){
                    LayerIndex=parentLayerIndex,
                    Index=Random.Shared.Next(ind[parentLayerIndex].Count)
                };
            })
            .ToArray();
        layers[indexOfLayerAddNodeTo].Add(
            Random.Shared.Choose(this.Nodes).Clone(parents)
        );

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        if ( !newChromosome.IsValid() )
            throw new Exception($"Created invalid chromosome in {this.GetType()}!");
        
        return newChromosome;
    }
}