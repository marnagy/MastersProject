public class RemoveLayerMutation : Mutation<CartesianChromosome>
{
    public RemoveLayerMutation(double probability): base(probability) { }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        var layers = ind.DeepCopyLayers();

        int indexOfLayerToDelete;
        // don't remove output layer
        indexOfLayerToDelete = Random.Shared.Next(layers.Count - 1);

        // remove layer
        layers.RemoveAt(indexOfLayerToDelete);

        // fix indices if needed
        // TODO: check if correct
        for (int layerIndex = indexOfLayerToDelete; layerIndex < layers.Count; layerIndex++)
        {
            foreach (var node in layers[layerIndex])
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
                            var newLayerIndex = Random.Shared.Next(layerIndex + 1); // include input layer
                            return new ParentIndices(){
                                LayerIndex=newLayerIndex,
                                Index=Random.Shared.Next(ind[newLayerIndex].Count)
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