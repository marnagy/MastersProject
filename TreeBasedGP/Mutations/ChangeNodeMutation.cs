/// <summary>
/// Change type of node functionality <b>inplace</b>.
/// </summary>
public class ChangeNodeMutation : Mutation<TreeChromosome>
{
    public readonly double PercentageToChange;
    public readonly IReadOnlyDictionary<NodeFunctionality, double> TerminalNodesProbabilities;
    public readonly IReadOnlyList<NodeFunctionality> TerminalNodes;
    public readonly IReadOnlyDictionary<NodeFunctionality, double> NonTerminalNodesProbabilities;
    public readonly IReadOnlyList<NodeFunctionality> NonTerminalNodes;
    public readonly double TerminalNodesProbability;
    private const int DefaultNewDepth = 2;
    private double[]? terminalNodesWeights;
    private double[]? nonTerminalNodesWeights;
    public ChangeNodeMutation(double probability, 
            double percentageToChange,
            double terminalNodesProbability,
            IReadOnlyDictionary<NodeFunctionality, double> terminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> terminalNodes,
            IReadOnlyDictionary<NodeFunctionality, double> nonTerminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> nonTerminalNodes): base(probability)
    {
        this.PercentageToChange = percentageToChange;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodes = terminalNodes;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.NonTerminalNodes = nonTerminalNodes;
    }
    public override TreeChromosome Mutate(TreeChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        if (rand_value < this.MutationProbability)
            this.Mutate(ind.RootNode, ind, genNum);

        return ind;
    }
    private void Mutate(TreeNodeMaster node, TreeChromosome ind, int genNum)
    {
        // don't mutate node
        if (Random.Shared.NextDouble() > this.PercentageToChange) // don't mutate
        {
            if (!node.HasChildren)
                return;
                
            for (int i = 0; i < node.Children.Length; i++)
            {
                this.Mutate(node.Children[i], ind, genNum);
            }
            return;
        }
            

        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            this.terminalNodesWeights ??= this.TerminalNodes
                .Select(node => this.TerminalNodesProbabilities[node])
                .ToArray();

            var chosenFunctionality = Random.Shared
                .Choose(
                    this.TerminalNodes,
                    weights: this.terminalNodesWeights
                );
            var prevFunctionality = node.Functionality;

            node.Functionality = chosenFunctionality;
            // changing to terminal node
            node.Children = null;
        }
        else
        {
            this.nonTerminalNodesWeights ??= this.NonTerminalNodes
                    .Select(node => this.NonTerminalNodesProbabilities[node])
                    .ToArray();

            // choose new non-terminal node
            var chosenFunctionality = Random.Shared
                .Choose(
                    this.NonTerminalNodes,
                    weights: this.nonTerminalNodesWeights
                );
            var prevFunctionality = node.Functionality;

            if (!prevFunctionality.NeedsChildren)
            {
                // create children
                node.Children = Enumerable.Range(0, TreeNodeMaster.ChildrenAmount)
                    .Select(_ => ind.CreateNewTreeFull(TreeChromosome.DefaultSubtreeDepth))
                    .ToArray();
            }
        }

    }
}