public class AddNodeToLayerMutation : Mutation<CartesianChromosome>
{
    private readonly IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> NodeCatalogue;
    private readonly IReadOnlyList<CartesianNode> Nodes;
    public AddNodeToLayerMutation(double probability,
        IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> nodeCatalogue, int? seed): base(probability, seed)
    {
        this.NodeCatalogue = nodeCatalogue;
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
        
        int indexOfLayerAddNodeTo;
        lock (this)
        {
            // don't change output layer
            indexOfLayerAddNodeTo = _rng.Next(layers.Count - 1);
        }

        var parents = Enumerable.Range(0, CartesianNode.ParentsAmount)
            .Select(_ => { 
                // include input layer
                var parentLayerIndex = this._rng.Next(indexOfLayerAddNodeTo + 1);
                return new ParentIndices(){
                    LayerIndex=parentLayerIndex,
                    Index=this._rng.Next(ind[parentLayerIndex].Count)
                };
            })
            .ToArray();
        layers[indexOfLayerAddNodeTo].Add(
            this._rng.Choose(this.Nodes).Clone(parents)
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