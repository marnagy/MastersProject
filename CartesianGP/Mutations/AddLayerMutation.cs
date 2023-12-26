public class AddLayerMutation : Mutation<CartesianChromosome>
{
    private const int _newLayerSize = 3;
    private readonly IReadOnlyDictionary<int, IList<CartesianNode>> nodeCatalogue;
    private readonly IReadOnlyList<CartesianNode> Nodes;
    public AddLayerMutation(double probability, Dictionary<int, IList<CartesianNode>> nodeCatalogue): base(probability)
    {
        this.nodeCatalogue = nodeCatalogue;
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

        int indexToInsertLayerTo;
        // output layer cannot change
        // if indexToInsertLayerTo == ^1 then output layer is moved by 1 index to the back
        indexToInsertLayerTo = Random.Shared.Next(layers.Count);

        var newLayer = Enumerable.Range(0, _newLayerSize)
            .Select(_ => {
                ParentIndices[] parents = Enumerable
                    .Range(0, CartesianNode.ParentsAmount)
                    .Select(_ => {
                        // Layer index includes Input layer
                        int layerIndex = Random.Shared.Next(indexToInsertLayerTo + 1);
                        return new ParentIndices(){
                            LayerIndex=layerIndex,
                            // should be fine 
                            Index=Random.Shared.Next(ind[layerIndex].Count)
                        };
                    })
                    .ToArray();
                return Random.Shared.Choose(this.Nodes).Clone(parents);
            })
            .ToList();

        // TODO: re-index parents for nodes AFTER new layer that point to nodes AFTER new layer
        foreach (var node in layers[indexToInsertLayerTo..].SelectMany(nodes => nodes))
        {
            ParentIndices[] nodeParents = node.Parents;
            ParentIndices[] fixedParents = nodeParents
                .Select(parent => parent.LayerIndex >= indexToInsertLayerTo
                    // shift LayerIndex further back (+1) 
                    ? new ParentIndices(){
                        LayerIndex=parent.LayerIndex+1,
                        Index=parent.Index
                    }
                    : parent.Clone())
                .ToArray();
        }
        
        layers.Insert(indexToInsertLayerTo, newLayer);

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        if (!newChromosome.IsValid())
            throw new Exception("Created invalid choromosome in AddLayerMutation");

        return newChromosome;
    }
}