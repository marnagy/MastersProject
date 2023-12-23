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
            IReadOnlyList<TreeNode> nonTerminalNodes,
            int? seed): base(probability, seed)
    {
        this.PercentageToChange = percentageToChange;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodes = terminalNodes;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.NonTerminalNodes = nonTerminalNodes;
    }
    private bool ShouldChange()
    => _rng.NextDouble() < this.MutationProbability;
    public override TreeChromosome Mutate(TreeChromosome ind, int genNum)
    {
        // TODO: Mutation is slowing down the GA
        // !: HOW?
        double rand_value;
        lock (this)
        {
            rand_value = this._rng.NextDouble();
        }

        if (rand_value < this.MutationProbability)
            return ind.Clone();

        lock (this)
        {
            return ind.Clone(
                this.Mutate(ind._rootNode, ind, genNum)
            );
        }
    }
    private TreeNode Mutate(TreeNode origNode, TreeChromosome ind, int genNum)
    {
        TreeNode[]? children = origNode.Children?
            .Select(childNode => this.Mutate(childNode, ind, genNum))
            .ToArray();

        // don't mutate node
        if ( !(this._rng.NextDouble() < this.PercentageToChange))
            return origNode.Clone(children);

        if (this._rng.NextDouble() < this.TerminalNodesProbability)
        {
            // choose new terminal node
            return this._rng
                .Choose(this.TerminalNodes)
                .Clone(children: null);
        }
        else
        {
            // choose new non-terminal node
            return this._rng
                .Choose(this.NonTerminalNodes)
                .Clone(
                    origNode.HasChildren
                        ? children
                        // create new children
                        : Enumerable.Range(0, TreeNode.ChildrenAmount)
                            .Select(_ => ind.CreateNewTreeFull(ChangeNodeMutation.DefaultNewDepth))
                            .ToArray()

                );
        }

    }
}