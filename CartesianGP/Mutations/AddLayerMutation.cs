public class AddLayerMutation : Mutation<CartesianChromosome>
{
    private const int _newLayerSize = 3;
    private readonly IReadOnlyDictionary<int, IList<CartesianNode>> nodeCatalogue;
    private readonly IReadOnlyList<CartesianNode> Nodes;
    public AddLayerMutation(double probability, Dictionary<int, IList<CartesianNode>> nodeCatalogue, int? seed = null): base(probability, seed)
    {
        this.nodeCatalogue = nodeCatalogue;
        this.Nodes = nodeCatalogue.Keys
            .SelectMany(arity => nodeCatalogue[arity])
            .ToArray();
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        lock (this)
        {
            rand_value = this._rng.NextDouble();
        }

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        var layers = ind.DeepCopyLayers();

        int indexToInsertLayerTo;
        lock(this)
        {
            // output layer cannot change
            // if indexToInsertLayerTo == ^1 then output layer is moved by 1 index to the back
            indexToInsertLayerTo = _rng.Next(layers.Count);
        }

        var newLayer = Enumerable.Range(0, _newLayerSize)
            .Select(_ => {
                lock(this)
                {
                    ParentIndices[] parents = Enumerable
                        .Range(0, CartesianNode.ParentsAmount)
                        .Select(_ => {
                            // !: needed lock is 5 lines higher
                            // Layer index includes Input layer
                            int layerIndex = this._rng.Next(indexToInsertLayerTo + 1);
                            return new ParentIndices(){
                                LayerIndex=layerIndex,
                                // should be fine 
                                Index=this._rng.Next(ind[layerIndex].Count)
                            };
                        })
                        .ToArray();
                    return this._rng.Choose(this.Nodes).Clone(parents);
                };
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