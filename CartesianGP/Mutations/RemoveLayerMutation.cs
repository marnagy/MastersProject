public class RemoveLayerMutation : Mutation<CartesianChromosome>
{
    public RemoveLayerMutation(double probability, int? seed): base(probability, seed) { }
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

        int indexOfLayerToDelete;
        lock (this)
        {
            indexOfLayerToDelete = _rng.Next(layers.Count);
        }

        // remove layer
        layers.RemoveAt(indexOfLayerToDelete);

        // fix indices if needed
        for (int i = indexOfLayerToDelete; i < layers.Count; i++)
        {
            foreach (var node in layers[i])
            {
                var parents = node.Parents;

                var newParents = parents
                    .Select(parent => {
                        // only move index
                        if (parent.LayerIndex > indexOfLayerToDelete)
                        {
                            return new ParentIndices(){
                                LayerIndex=parent.LayerIndex - 1,
                                Index=parent.Index
                            };
                        }
                        // choose new parent from NOT ALTERED layers
                        else if (parent.LayerIndex == indexOfLayerToDelete)
                        {
                            var newLayerIndex = this._rng.Next(indexOfLayerToDelete + 1); // include input layer
                            return new ParentIndices(){
                                LayerIndex=newLayerIndex,
                                Index=this._rng.Next(ind[newLayerIndex].Count)
                            };
                        }
                        // else leave index as previously
                        else
                            return parent.Clone();
                    })
                    .ToArray();
                
                node.Parents = newParents;
            }
        }

        var newChromosome = new CartesianChromosome(ind.InputsAmount, layers);

        if (!newChromosome.IsValid())
            throw new Exception("Created invalid choromosome in RemoveLayerMutation");
        
        return newChromosome;
    }
}