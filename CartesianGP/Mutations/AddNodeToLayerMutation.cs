public class AddNodeToLayerMutation : Mutation<CartesianChromosome>
{
    private readonly double TerminalNodesProbability;
    private readonly IReadOnlyDictionary<CartesianNode, double> TerminalNodesProbabilities;
    private IReadOnlyList<CartesianNode> TerminalNodes;
    private readonly IReadOnlyDictionary<CartesianNode, double> NonTerminalNodesProbabilities;
    private IReadOnlyList<CartesianNode> NonTerminalNodes;
    public AddNodeToLayerMutation(double probability,
        double terminalNodesProbability,
        IReadOnlyDictionary<CartesianNode, double> terminalNodesProbabilities,
        IReadOnlyDictionary<CartesianNode, double> nonTerminalNodesProbabilities): base(probability)
    {
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.TerminalNodes = terminalNodesProbabilities.Keys.ToArray();
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.NonTerminalNodes = nonTerminalNodesProbabilities.Keys.ToArray();
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        var layers = ind.DeepCopyLayers();
        
        int indexOfLayerAddNodeTo;
        // don't change output layer
        indexOfLayerAddNodeTo = Random.Shared.Next(layers.Count - 1);

        var parents = CartesianChromosome.ChooseParents(
            inputsAmount: ind.InputsAmount,
            internalLayers: layers,
            internalLayerIndex: indexOfLayerAddNodeTo
        );
        IReadOnlyList<CartesianNode> nodes;
        IReadOnlyList<double> nodeWeights;

        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            nodes = this.TerminalNodes;
            nodeWeights = this.TerminalNodes
                .Select(node => this.TerminalNodesProbabilities[node])
                .ToArray();
        }
        else
        {
            nodes = this.NonTerminalNodes;
            nodeWeights = this.NonTerminalNodes
                .Select(node => this.NonTerminalNodesProbabilities[node])
                .ToArray();
        }
        layers[indexOfLayerAddNodeTo].Add(
            Random.Shared.Choose(nodes, nodeWeights).Clone(parents)
        );

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        if ( !newChromosome.IsValid() )
            throw new Exception($"Created invalid chromosome in {this.GetType()}!");
        
        return newChromosome;
    }
}