public class CartesianChromosome : Chromosome<CartesianChromosome>
{
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
            .Select(_ => new ValueNode(0d, CartesianNode.GetEmptyParents()))
            .ToArray();
        this.Layers = layers;
    }
    public IReadOnlyList<CartesianNode> this[int i]
    {
        get => i == 0 ? Inputs : Layers[i-1];
    }

    public static CartesianChromosome CreateNewRandom(int[] layerSizes,
        IReadOnlyList<CartesianNode> nodeCatalogue)
    {
        var rng = new Random();

        int inputsAmount = layerSizes[0];
        List<List<CartesianNode>> layers = new List<List<CartesianNode>>();
        for (int currentLayer = 1; currentLayer < layerSizes.Length; currentLayer++)
        {
            layers.Add(new List<CartesianNode>(layerSizes[currentLayer]));
            for (int j = 0; j < layerSizes[currentLayer]; j++)
            {
                // choose 2 parents
                List<ParentIndices> parents = new List<ParentIndices>(CartesianNode.ParentsAmount);
                for (int k = 0; k < CartesianNode.ParentsAmount; k++)
                {
                    int layerIndex = rng.Next(currentLayer);
                    int nodeIndex = rng.Next(layerSizes[layerIndex]);
                    parents.Add( new ParentIndices{
                        LayerIndex = layerIndex,
                        Index = nodeIndex
                    } );
                }
                // choose node and create clone with new parents
                CartesianNode templateNode = nodeCatalogue[
                    rng.Next(nodeCatalogue.Count)
                ];
                layers[currentLayer - 1].Add(
                    templateNode.Clone(
                        parents.ToArray()
                    )
                );
            }
        }

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
                node.Compute(this);
            }
        }

        return this.Layers[^1]
            .Select(node => node.Result);
            // .ToArray(); // maybe remove for optimalization?
    }

    public override CartesianChromosome Clone()
        => new CartesianChromosome(
            inputsAmount: this.Inputs.Length,
            layers: this.Layers
                .Select(layer => layer
                    .Select(node => node.Clone())    
                    .ToList()
                )
                .ToList()
        );
    
    public List<List<CartesianNode>> DeepCopyLayers()
    {
        return this.Layers
            .Select(layer => layer
                .Select(node => node.Clone())
                .ToList()
            )
            .ToList();
    }

    public IEnumerable<int> GetLayerSizes()
        => this.Layers.Select(layer => layer.Count);

    public override CartesianChromosome CreateNew()
    {
        throw new Exception($"Not implemented for CartesianChromosome. Please use CartesianChromosome.CreateNewRandom(...)");
    }
}