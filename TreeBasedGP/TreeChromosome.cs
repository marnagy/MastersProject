public class TreeChromosome : Chromosome<TreeChromosome>
{
    public readonly TreeNode _rootNode;
    public readonly int Depth;
    public static int DefaultDepth = 2;
    private readonly int? _seed;
    public readonly IReadOnlyDictionary<TreeNode, double> TerminalNodesProbabilities;
    public readonly IReadOnlyDictionary<TreeNode, double> NonTerminalNodesProbabilities;
    public readonly double TerminalNodesProbability;
    public TreeChromosome(TreeNode rootNode, double terminalNodesProbability,
            IReadOnlyDictionary<TreeNode, double> terminalNodesProbabilities,
            IReadOnlyDictionary<TreeNode, double> nonTerminalNodesProbabilities)
    {
        this._rootNode = rootNode;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
    }
    public override TreeChromosome Clone()
    => this.Clone(this._rootNode);
    public TreeChromosome Clone(TreeNode rootNode)
    => new TreeChromosome(
        rootNode.Clone(),
        this.TerminalNodesProbability,
        this.TerminalNodesProbabilities,
        this.NonTerminalNodesProbabilities
    );
    /// <summary>
    /// this method creates full tree with depth DefaultDepth by default by default.
    /// </summary>
    /// <returns>New full TreeChromosome</returns>
    public override TreeChromosome CreateNew()
    => this.Clone(
        this.CreateNewTreeFull(TreeChromosome.DefaultDepth)
    );
    public TreeNode CreateNewTreeFull(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depth);

        if (depth == 1)
            return Random.Shared.Choose(
                this.TerminalNodesProbabilities.Keys.ToArray()
            )
            .Clone(children: null);
        
        // choose non-terminal node
        TreeNode[] children = Enumerable.Range(0, TreeNode.ChildrenAmount)
            .Select(_ => this.CreateNewTreeGrow(depth - 1))
            .ToArray();
        return Random.Shared
            .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray())
            .Clone(children);
    }
    /// <summary>
    /// Recursive function that creates tertiary tree with max depth of argument depth.
    /// </summary>
    /// <param name="depth">Maximum depth of Tree. <b>Root node is considered a layer 1.</b></param>
    /// <returns></returns>
    public TreeNode CreateNewTreeGrow(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depth);

        if (depth == 1)
            return Random.Shared.Choose(
                    this.TerminalNodesProbabilities.Keys.ToArray()
            )
            .Clone(children: null);

        // else
        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            // choose a terminal node
            return Random.Shared
                .Choose(this.TerminalNodesProbabilities.Keys.ToArray())
                .Clone(children: null);
        }
        else
        {
            // choose non-terminal node
            TreeNode[] children = Enumerable.Range(0, TreeNode.ChildrenAmount)
                .Select(_ => this.CreateNewTreeGrow(depth - 1))
                .ToArray();
            return Random.Shared
                .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray())
                .Clone(children);
        }
    }
    /// <summary>
    /// Expects InputNodes to be set accordingly to 1 input
    /// </summary>
    /// <returns></returns>
    public double ComputeResult()
    => this._rootNode.Compute();
    public override bool IsValid()
    => TreeChromosome.IsValid(this._rootNode);
    private static bool IsValid(TreeNode node)
    {
        if (node is ValueNode valNode)
        {
            return !node.HasChildren;
        }
        else
            return
                node.HasChildren &&
                    node.Children
                        .All(childNode => TreeChromosome.IsValid(childNode));
    }
    public string GetRepresentation()
    => this._rootNode.Representation();
    public override string ToString()
    {
        return $"{this.GetType()}[ {this._rootNode} ]";
    }
}