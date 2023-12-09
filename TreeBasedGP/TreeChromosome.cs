public class TreeChromosome : Chromosome<TreeChromosome>
{
    public readonly TreeNode _rootNode;
    public readonly int Depth;
    public const int DefaultDepth = 3;
    private readonly Random _rng;
    public static IReadOnlyList<TreeNode> TerminalNodes;
    public static IReadOnlyList<TreeNode> NonTerminalNodes;
    public static double TerminalNodesProbability;
    public TreeChromosome(TreeNode rootNode, int? seed = null)
    {
        if (TreeChromosome.Nodes is null)
            throw new Exception($"{nameof(TreeChromosome.Nodes)} needs to be set to not null.")

        this._rootNode = rootNode;
        this._rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }
    public override TreeChromosome Clone()
    => new TreeChromosome(this._rootNode.Clone());

    /// <summary>
    /// this method creates full tree with depth DefaultDepth by default by default.
    /// </summary>
    /// <returns>New full TreeChromosome</returns>
    public override TreeChromosome CreateNew()
    => new TreeChromosome(
        this.CreateNewTreeFull(TreeChromosome.DefaultDepth)
    );
    /// <summary>
    /// Recursive function that creates tertiary tree with max depth of argument depth.
    /// </summary>
    /// <param name="depth">Maximum depth of Tree. <b>Root node is considered a layer 1.</b></param>
    /// <returns></returns>
    public TreeNode CreateNewTreeFull(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depth);

        if (depth == 1)
            lock (this)
            {
                return this._rng.Choose(TreeChromosome.TerminalNodes)
                    .Clone(children: null);
            }
        
        // else
        lock (this)
        {
            if (this._rng.NextDouble() < TreeChromosome.TerminalNodesProbability)
            {
                // choose a terminal
            }
            else
            {
                // choose non-terminal node
            }
        }

    }
    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}