/// <summary>
/// Splits Chromosomes using the same index of layers and fixes index
/// </summary>
public class FixedParentsCrossover : Crossover<CartesianChromosome>
{
    private static Random _rng = new();
    public override Tuple<CartesianChromosome, CartesianChromosome> Cross(CartesianChromosome ind1, CartesianChromosome ind2)
    {
        // TODO: choose index to split chromosomes (single for both)
        // This way there is no need for fixing layer numbers of parents.
        int count1 = ind1.GetLayerSizes().Count();
        int count2 = ind2.GetLayerSizes().Count();

        // subtract input layer
        int maxCommonIndex = Math.Min(count1, count2) - 1;

        int splittingIndex = _rng.Next(maxCommonIndex);

        var layers1 = ind1.DeepCopyLayers();
        var layers2 = ind2.DeepCopyLayers();

        var newLayers1 = layers1.GetRange(0, splittingIndex);
        newLayers1.AddRange(
            layers2.GetRange(splittingIndex, layers2.Count - splittingIndex)
        );
        var newLayers2 = layers2.GetRange(0, splittingIndex);
        newLayers2.AddRange(
            layers1.GetRange(splittingIndex, layers1.Count - splittingIndex)
        );

        // TODO: Fix index of parents within their layer
        foreach (var layers in new[] {newLayers1, newLayers2})
        {
            if ( layers is null )
                continue;
            foreach (var layer in layers)
            {
                foreach (var node in layer)
                {   
                    node.Parents = node.Parents
                        .Select(parent =>
                        {
                            var parentLayerIndex = parent.LayerIndex;
                            var fixedParentIndexWithinLayer = parent.Index;

                            // If the node is from bigger layer, fix to the last node.
                            if (fixedParentIndexWithinLayer >= layers[parentLayerIndex].Count)
                            {
                                fixedParentIndexWithinLayer = layers[parentLayerIndex].Count - 1;
                            }
                            return new ParentIndices() {
                                LayerIndex=parentLayerIndex,
                                Index=fixedParentIndexWithinLayer
                            };
                        })
                        .ToArray();
                }
            }
        }

        var newInd1 = new CartesianChromosome(
            ind1.InputsAmount,
            newLayers1
        );
        var newInd2 = new CartesianChromosome(
            ind1.InputsAmount,
            newLayers2
        );

        return new Tuple<CartesianChromosome, CartesianChromosome>(
            newInd1,
            newInd2
        );
    }
}