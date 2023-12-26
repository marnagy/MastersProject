public class ChangeNodeMutation : Mutation<TreeChromosome>
{
    public readonly double PercentageToChange;
    public readonly IReadOnlyDictionary<TreeNode, double> TerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNode> TerminalNodes;
    public readonly IReadOnlyDictionary<TreeNode, double> NonTerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNode> NonTerminalNodes;
    public readonly double TerminalNodesProbability;
    private const int DefaultNewDepth = 2;
    public ChangeNodeMutation(double probability, 
            double percentageToChange,
            double terminalNodesProbability,
            IReadOnlyDictionary<TreeNode, double> terminalNodesProbabilities,
            IReadOnlyList<TreeNode> terminalNodes,
            IReadOnlyDictionary<TreeNode, double> nonTerminalNodesProbabilities,
            IReadOnlyList<TreeNode> nonTerminalNodes): base(probability)
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
        // TODO: Mutation is slowing down the GA
        // !: HOW?
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        if (rand_value < this.MutationProbability)
            return ind.Clone();

        return ind.Clone(
            this.Mutate(ind._rootNode, ind, genNum)
        );
    }
    private TreeNode Mutate(TreeNode origNode, TreeChromosome ind, int genNum)
    {
        // don't mutate node
        if ( !(Random.Shared.NextDouble() < this.PercentageToChange))
            return origNode.Clone(
                origNode.Children?
                    .Select(childNode => this.Mutate(childNode, ind, genNum))
                    .ToArray()
            );

        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            // choose new terminal node
            return Random.Shared
                .Choose(this.TerminalNodes)
                .Clone(children: null);
        }
        else
        {
            // choose new non-terminal node
            return Random.Shared
                .Choose(this.NonTerminalNodes)
                .Clone(
                    origNode.HasChildren
                        ? origNode.Children
                        // create new children
                        : Enumerable.Range(0, TreeNode.ChildrenAmount)
                            .Select(_ => ind.CreateNewTreeFull(ChangeNodeMutation.DefaultNewDepth))
                            .ToArray()

                );
        }

    }
}