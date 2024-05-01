public class RemoveNodeFromLayerMutation : Mutation<CartesianChromosome>
{
    public RemoveNodeFromLayerMutation(double probability): base(probability) { }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();
        
        var layers = ind.DeepCopyLayers();
        
        int layerToRemoveNodeFrom;
        int indexOfNodeToRemove;
        // don't remove from output layer
        layerToRemoveNodeFrom = Random.Shared.Next(layers.Count - 1);
        indexOfNodeToRemove = Random.Shared.Next(layers[layerToRemoveNodeFrom].Count);

        // remove node
        layers[layerToRemoveNodeFrom].RemoveAt(indexOfNodeToRemove);

        // TODO: fix any indices in layers after chosen node
        foreach (var layer in layers[(layerToRemoveNodeFrom+1)..])
        {
            foreach (var node in layer)
            {
                var newParents = node.Parents
                    .Select(parent => {
                        // node points to different layer
                        // LayerIndex includes input layer
                        if (parent.LayerIndex != layerToRemoveNodeFrom + 1)
                            return parent.Clone();
                        else if (parent.Index < indexOfNodeToRemove)
                            return parent.Clone();
                        else if (parent.Index > indexOfNodeToRemove)
                            return new ParentIndices(){
                                LayerIndex=parent.LayerIndex,
                                Index=parent.Index-1
                            };
                        else // choose new random parent
                        {
                            return CartesianChromosome.ChooseParents(
                                ind.InputsAmount,
                                internalLayers: layers,
                                internalLayerIndex: layerToRemoveNodeFrom
                            )[0];
                        }
                    })
                    .ToArray();
                
                node.Parents = newParents;
            }
        }

        var newChromosome = new CartesianChromosome(ind.InputsAmount, layers);

        if (!newChromosome.IsValid())
            throw new Exception($"Created invalid choromosome in {this.GetType()}");
        
        return newChromosome;
    }
}