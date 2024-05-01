using System.Text;

public class CartesianChromosome : Chromosome<CartesianChromosome>
{
    /// <summary>
    /// Layer of ValueNode nodes representing inputs. ParentIndices set to <b>-1</b>. 
    /// </summary>
    private readonly InputNode[] Inputs;
    public double Score { get; internal set; }
    /// <summary>
    /// Internal layers <b>excluding</b> Input nodes.
    /// </summary>
    private List<List<CartesianNode>> Layers;
    /// <summary>
    /// Create new random individual.
    /// </summary>
    /// <param name="hiddenLayerSizes">First is input layer, last is number of outputs.</param>
    /// <param name="nodeCatalogue">Set of possible Nodes</param>
    public CartesianChromosome(int inputsAmount, List<List<CartesianNode>> layers, double? previousFitness = null, double? previousScore = null)
    {
        this.Inputs = Enumerable.Range(0, inputsAmount)
            .Select(i => new InputNode(0d, inputIndex: i, CartesianNode.GetEmptyParents()))
            .ToArray();
        this.Layers = layers;
        if (previousFitness.HasValue)
            this.Fitness = previousFitness.Value;
        if (previousScore.HasValue)
            this.Score = previousScore.Value;
    }
    public IReadOnlyList<CartesianNode> this[int i]
    {
        get => i == 0 ? Inputs : Layers[i - 1];
    }

    public static CartesianChromosome CreateNewRandom(int[] layerSizes,
        double terminalNodesProbability,
        IReadOnlyDictionary<CartesianNode, double> terminalNodesProbabilities,
        IReadOnlyDictionary<CartesianNode, double> nonTerminalNodesProbabilities
        )
    {
        var now = DateTime.UtcNow;
        IReadOnlyList<CartesianNode> terminalNodes = terminalNodesProbabilities.Keys.ToArray();
        IReadOnlyList<double> terminalNodesWeights = terminalNodes
            .Select(node => terminalNodesProbabilities[node])
            .ToArray();
        IReadOnlyList<CartesianNode> nonTerminalNodes = nonTerminalNodesProbabilities.Keys.ToArray();
        IReadOnlyList<double> nonTerminalNodesWeights = nonTerminalNodes
            .Select(node => nonTerminalNodesProbabilities[node])
            .ToArray();

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
                    int layerIndex = Random.Shared.Next(currentLayer);
                    int nodeIndex;
                    if (layerIndex == 0)
                    {
                        // random from input layer
                        nodeIndex = Random.Shared.Next(inputsAmount);
                    }
                    else
                    {
                        // random from other layers
                        nodeIndex = Random.Shared.Next(layers[layerIndex - 1].Count);
                    }
                    parents.Add(new ParentIndices
                    {
                        LayerIndex = layerIndex,
                        Index = nodeIndex
                    });
                }

                CartesianNode newNode;
                IReadOnlyList<CartesianNode> nodes;
                IReadOnlyList<double> nodesWeights;
                if (Random.Shared.NextDouble() < terminalNodesProbability)
                {
                    nodes = terminalNodes;
                    nodesWeights = terminalNodesWeights;
                }
                else
                {
                    nodes = nonTerminalNodes;
                    nodesWeights = nonTerminalNodesWeights;
                }
                newNode = Random.Shared.Choose(nodes, nodesWeights)
                    .Clone(parents.ToArray());

                layers[currentLayer - 1].Add(
                    newNode
                );
            }
        }

        return new CartesianChromosome(inputsAmount, layers);
    }

    public IList<double> ComputeResult(double[] input)
    {
        if (this.Inputs.Length != input.Length)
            throw new ArgumentException($"Invalid number of inputs. Expected {this.Inputs.Length}, got {input.Length}");

        // set input nodes
        for (int i = 0; i < input.Length; i++)
        {
            this.Inputs[i].Value = input[i];
        }

        return this.Layers[^1]
            .Select(node => node.Compute(this))
            .ToArray();
    }

    public override CartesianChromosome Clone(bool preserveFitness = false)
        => new CartesianChromosome(
            inputsAmount: this.Inputs.Length,
            layers: this.Layers
                .Select(layer => layer
                    .Select(node => node.Clone())
                    .ToList()
                )
                .ToList(),
            previousFitness: this.Fitness,
            previousScore: this.Score
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
    public override bool IsValid() => CartesianChromosome.IsValid(this);

    public static bool IsValid(CartesianChromosome chromosome, bool verbose = false)
    {
        bool isValid = true;

        if (verbose)
        {
            System.Console.WriteLine("Checking chromosome:");
            System.Console.WriteLine(chromosome);
        }
        
        // inputs layer
        ParentIndices invalidParents = ParentIndices.GetInvalid();
        foreach (ValueNode valNode in chromosome.Inputs)
        {
            isValid = isValid && valNode.Parents.All(par => par == invalidParents);
        }

        if (verbose)
            System.Console.Error.WriteLine($"Inputs' parents valid? {isValid}");

        // other layers
        for (int layerIndex = 0; layerIndex < chromosome.Layers.Count; layerIndex++)
        {
            var layer = chromosome.Layers[layerIndex];
            if (verbose)
                System.Console.Error.WriteLine($"Validating internal layer {layerIndex}");
            foreach (CartesianNode node in layer)
            {
                if (verbose)
                {
                    System.Console.Error.WriteLine($"Checking node of type {node.GetType()}");
                    System.Console.Error.WriteLine($"Node parents:{node.Parents.Stringify()}");
                    System.Console.Error.WriteLine($"Testing nodeINLayerTest...");
                }
                var nodeInLayerTest = node.Parents.All(par => par.Index < chromosome[par.LayerIndex].Count);
                if (verbose)
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
    /// <summary>
    /// Get sizes of layers <b>excluding</b> input layer.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetLayerSizes()
        => this.Layers.Select(layer => layer.Count);
    
    public int GetDepth()
    => 1 + this.Layers.Count;

    public int InputsAmount => this.Inputs.Length;

    public override CartesianChromosome CreateNew()
    {
        throw new Exception($"Not implemented for CartesianChromosome. Please use CartesianChromosome.CreateNewRandom(...)");
    }
    public static ParentIndices[] ChooseParents(int inputsAmount, List<List<CartesianNode>> internalLayers, int internalLayerIndex)
    => Enumerable.Range(0, CartesianNode.ParentsAmount)
        .Select(_ => { 
            // include input layer
            var parentLayerIndex = Random.Shared.Next(internalLayerIndex + 1);
            if (parentLayerIndex == 0)
            {
                // choose from inputs
                return new ParentIndices(){
                    LayerIndex=parentLayerIndex,
                    Index=Random.Shared.Next(inputsAmount)
                };
            }
            return new ParentIndices(){
                LayerIndex=parentLayerIndex,
                Index=Random.Shared.Next(internalLayers[parentLayerIndex-1].Count)
            };
        })
        .ToArray();
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("CartesianChromosome:");
        // inputs
        sb.Append("[");
        sb.Append(string.Join(", ", this.Inputs.AsEnumerable()));
        sb.AppendLine("]");
        // layers
        foreach (var layer in this.Layers)
        {
            sb.Append("[");
            sb.Append(string.Join(", ", layer));
            sb.AppendLine("]");
        }
        return sb.ToString();
    }

    internal string GetRepresentation()
    {
        var sb = new StringBuilder();
        foreach (var outputNode in this.Layers[^1])
        {
            outputNode.GetRepresentation(sb, this);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}