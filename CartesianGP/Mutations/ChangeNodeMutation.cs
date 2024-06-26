public class ChangeNodeMutation : Mutation<CartesianChromosome>
{
    private readonly double PercentageToChange;
    private readonly double TerminalNodesProbability;
    private readonly IReadOnlyDictionary<CartesianNode, double> NonTerminalNodesProbabilities;
    private readonly IReadOnlyList<CartesianNode> NonTerminalNodes;
    private readonly IReadOnlyDictionary<CartesianNode, double> TerminalNodesProbabilities;
    private readonly IReadOnlyList<CartesianNode> TerminalNodes;

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
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            for (int j = 0; j < layers[layerIndex].Count; j++)
            {
                if (shouldNodeMutate[layerIndex][j])
                {
                    // choose new random node, preserve parents
                    layers[layerIndex][j] = this.PerformMutation(
                        previousNode: layers[layerIndex][j],
                        layerIndex: layerIndex,
                        nodeIndex: j
                    );
                }
            }
        }

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        return newChromosome;
    }
    private CartesianNode PerformMutation(CartesianNode previousNode, int layerIndex, int nodeIndex)
    {
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
            // exchange for a non-terminal node
            nodes = this.NonTerminalNodes;
            nodesWeights = this.NonTerminalNodes
                    .Select(termNode => this.NonTerminalNodesProbabilities[termNode])
                    .ToArray();
        }

        var newNode = Random.Shared.Choose(
            nodes,
            nodesWeights
        );
        return newNode.Clone(previousNode.Parents);
    }
}