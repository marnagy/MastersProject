public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private readonly double PercentageToChange;
    private readonly double TerminalNodesProbability;

    // private readonly IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> NodeCatalogue;
    private readonly IReadOnlyDictionary<CartesianNode, double> NonTerminalNodesProbabilities;
    private readonly IReadOnlyList<CartesianNode> NonTerminalNodes;
    private readonly IReadOnlyDictionary<CartesianNode, double> TerminalNodesProbabilities;
    private readonly IReadOnlyList<CartesianNode> TerminalNodes;
    // private readonly IReadOnlyList<CartesianNode> Nodes;

    public ChangeNodeMutation(double chromosomePercentageToChange, double probability,
            double terminalNodesProbability,
            IReadOnlyDictionary<CartesianNode, double> nonTerminalNodesProbabilities,
            IReadOnlyDictionary<CartesianNode, double> terminalNodesProbabilities) : base(probability)
    {
        this.PercentageToChange = chromosomePercentageToChange;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.NonTerminalNodes = this.NonTerminalNodesProbabilities.Keys.ToArray();
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.TerminalNodes = this.TerminalNodesProbabilities.Keys.ToArray();
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        // mutate
        IList<IList<bool>> shouldNodeMutate;
        shouldNodeMutate = ind.GetLayerSizes()
            .Select(layerSize => Enumerable.Range(0, layerSize)
                .Select(_ => Random.Shared.NextDouble())
                .Select(prob => prob < this.PercentageToChange)
                .ToArray()
            )
            .ToArray();

        var layers = ind.DeepCopyLayers();
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                if (shouldNodeMutate[i][j])
                {
                    // choose new random node, preserve parents
                    System.Console.Error.WriteLine($"PreviousParents: {layers[i][j].Parents.Stringify()}");
                    layers[i][j] = this.PerformMutation(ind, layerIndex: i, nodeIndex: j);
                    // layers[i][j] = Random.Shared.Choose(this.Nodes).Clone(layers[i][j].Parents);
                    System.Console.Error.WriteLine($"Parents after mutation: {layers[i][j].Parents.Stringify()}");
                }
            }
        }

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        System.Console.Error.WriteLine($"ChangeNodeCreated valid chromosome? {CartesianChromosome.IsValid(newChromosome)}");

        if ( !CartesianChromosome.IsValid(newChromosome) )
            throw new Exception($"Created invalid chromosome in {this.GetType()}!");

        return newChromosome;
    }
    private CartesianNode PerformMutation(CartesianChromosome ind, int layerIndex, int nodeIndex)
    {
        var previousNode = ind[layerIndex][nodeIndex];
        IReadOnlyList<CartesianNode> nodes;
        IReadOnlyList<double> nodesWeights;

        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            // exchange for a terminal node
            nodes = this.TerminalNodes;
            nodesWeights = this.TerminalNodes
                    .Select(termNode => this.TerminalNodesProbabilities[termNode])
                    .ToArray();
        }
        else
        {
            nodes = this.NonTerminalNodes;
            nodesWeights = this.NonTerminalNodes
                    .Select(termNode => this.NonTerminalNodesProbabilities[termNode])
                    .ToArray();
            // exchange for a non-terminal node
        }

        var newNode = Random.Shared.Choose(
            nodes,
            nodesWeights
        );
        return newNode.Clone(previousNode.Parents);
    }
}