public class CartesianChromosome : Chromosome<CartesianChromosome>
{
    private readonly ValueNode[] Inputs;
    private List<List<CartesianNode>> Layers;
    /// <summary>
    /// Create new random individual.
    /// </summary>
    /// <param name="hiddenLayerSizes">First is input layer, last is number of outputs.</param>
    /// <param name="nodeCatalogue">Set of possible Nodes</param>
    public CartesianChromosome(int inputsAmount, List<List<CartesianNode>> layers)
    {
        this.Inputs = Enumerable.Range(0, inputsAmount)
            .Select(_ => new ValueNode(0d, CartesianNode.GetEmptyParents()))
            .ToArray();
        this.Layers = layers;
    }
    public IReadOnlyList<CartesianNode> this[int i]
    {
        get => i == 0 ? Inputs : Layers[i - 1];
    }

    public static CartesianChromosome CreateNewRandom(int[] layerSizes,
        Dictionary<int, IList<CartesianNode>> nodeCatalogue)
    {
        // !: Fix creation of chromosome
        // invalid LayerIndex in nodes
        var now = DateTime.UtcNow;
        var rng = new Random(now.GetTimestamp());

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
                    int nodeIndex;
                    if (layerIndex == 0)
                    {
                        // random from input layer
                        nodeIndex = rng.Next(inputsAmount);
                    }
                    else
                    {
                        // random from other layers
                        nodeIndex = rng.Next(layers[layerIndex - 1].Count);
                    }
                    System.Console.Error.WriteLine($"Choosing parent LayerIndex {layerIndex} with Index {nodeIndex}");
                    parents.Add(new ParentIndices
                    {
                        LayerIndex = layerIndex,
                        Index = nodeIndex
                    });
                }
                // choose node and create clone with new parents
                // choose random arity
                var arities = nodeCatalogue.Keys.ToArray();
                var arityIndex = rng.Choose(arities);
                CartesianNode templateNode = rng.Choose(nodeCatalogue[
                    arityIndex
                ]);
                CartesianNode newNode = templateNode.Clone(
                    parents.ToArray()
                );
                System.Console.Error.WriteLine($"Creating node with parents:");
                foreach (ParentIndices par in newNode.Parents)
                {
                    System.Console.Error.WriteLine($"Choosing parent LayerIndex {par.LayerIndex} with Index {par.Index}");
                }
                layers[currentLayer - 1].Add(
                    newNode
                );
            }
        }

        return new CartesianChromosome(inputsAmount, layers);
    }

    public IEnumerable<double> ComputeResult(double[] input)
    {
        if (this.Inputs.Length != input.Length)
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
    /// <summary>
    /// Deep copy layers of Cartesian chromosome. Does NOT include input layer.
    /// </summary>
    /// <returns></returns>
    public List<List<CartesianNode>> DeepCopyLayers()
    {
        return this.Layers
            .Select(layer => layer
                .Select(node => node.Clone())
                .ToList()
            )
            .ToList();
    }

    public static bool IsValid(CartesianChromosome chromosome)
    {
        bool isValid = true;

        // inputs layer
        foreach (ValueNode valNode in chromosome.Inputs)
        {
            isValid = isValid && valNode.Parents.All(par => par == ParentIndices.GetInvalid());
        }

        // other layers
        for (int layerIndex = 0; layerIndex < chromosome.Layers.Count; layerIndex++)
        {
            var layer = chromosome.Layers[layerIndex];
            System.Console.Error.WriteLine($"Validating layer {layerIndex}");
            foreach (CartesianNode node in layer)
            {
                System.Console.Error.WriteLine("Node parents:");
                foreach (var par in node.Parents)
                {
                    System.Console.Error.WriteLine($"LayerIndex: {par.LayerIndex}, Index: {par.Index}");
                }
                System.Console.Error.WriteLine($"Testing nodeINLayerTest...");
                var nodeInLayerTest = node.Parents.All(par => par.Index < chromosome[par.LayerIndex].Count);
                System.Console.Error.WriteLine($"Testing layerTest...");
                var layerTest = node.Parents.All(par => par.LayerIndex < layerIndex + 1);
                isValid = isValid
                    && nodeInLayerTest && layerTest; // include Inputs layer
            }

            if (!isValid)
                break;
        }

        return isValid;
    }

    public IEnumerable<int> GetLayerSizes()
        => this.Layers.Select(layer => layer.Count);

    public int InputsAmount => this.Inputs.Length;

    public override CartesianChromosome CreateNew()
    {
        throw new Exception($"Not implemented for CartesianChromosome. Please use CartesianChromosome.CreateNewRandom(...)");
    }
}