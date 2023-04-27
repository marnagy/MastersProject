public class CartesianChromosome : Chromosome<CartesianChromosome>, ICloneable
{
    private static bool canOverwriteCatalogue = true;
    //private static IReadOnlyList<CartesianNode>? NodeCatalogue;
    private readonly ValueNode[] Inputs;
    private List<List<CartesianNode>> Layers;
    /// <summary>
    /// Create new random individual.
    /// </summary>
    /// <param name="hiddenLayerSizes">First is input layer, last is number of outputs.</param>
    /// <param name="nodeCatalogue">Set of possible Nodes</param>
    private CartesianChromosome(int inputsAmount, List<List<CartesianNode>> layers)
    {
        this.Inputs = Enumerable.Range(0, inputsAmount)
            .Select(_ => new ValueNode(0d))
            .ToArray();
        this.Layers = layers;
    }
    // public static void SetNodeCatalogue(IReadOnlyList<CartesianNode> nodeCatalogue)
    // {
    //     if ( CartesianChromosome.canOverwriteCatalogue )
    //     {
    //         CartesianChromosome.NodeCatalogue = nodeCatalogue;
    //         CartesianChromosome.canOverwriteCatalogue = false;
    //     }
        
    // }

    public static CartesianChromosome CreateNewRandom(int[] layerSizes,
        IReadOnlyList<CartesianNode> nodeCatalogue)
    {
        var rng = new Random();

        int inputsAmount = layerSizes[0];
        List<List<CartesianNode>> layers = new List<List<CartesianNode>>();
        for (int i = 0; i < layerSizes.Length; i++)
        {
            
        }
         Enumerable.Range(0, layerSizes.Length)
            .Select(i => Enumerable.Range(0, layerSizes[i])
                    .Select(_ => nodeCatalogue[
                        rng.Next(nodeCatalogue.Count)
                    ])
                    .ToList()
            )
            .ToList();

        return new CartesianChromosome(inputsAmount, layers);
    }

    public IEnumerable<double> ComputeResult(double[] input)
    {
        if ( this.Inputs.Length != input.Length )
            throw new ArgumentException($"Invalid number of inputs. Expected {this.Inputs.Length}, got {input.Length}");
        
        for (int i = 0; i < input.Length; i++)
        {
            this.Inputs[i].Value = input[i];
        }

        foreach (var layer in this.Layers)
        {
            foreach (var node in layer)
            {
                node.Compute();
            }
        }

        return this.Layers[^1]
            .Select(node => node.Result);
            // .ToArray(); // maybe remove for optimalization?
    }

    public override Chromosome<CartesianChromosome> Clone()
    {
        throw new NotImplementedException();
    }

    public override CartesianChromosome CreateNew()
    {
        throw new NotImplementedException();
    }

    object ICloneable.Clone()
    {
        throw new NotImplementedException();
    }
}