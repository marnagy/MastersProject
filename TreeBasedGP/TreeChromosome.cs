public class TreeChromosome : Chromosome<TreeChromosome>
{
    public readonly TreeNode _rootNode;
    public readonly int Depth;
    public const int DefaultDepth = 3;
    private readonly Random _rng;
    private readonly int? _seed;
    public readonly IReadOnlyDictionary<TreeNode, double> TerminalNodesProbabilities;
    public readonly IReadOnlyDictionary<TreeNode, double> NonTerminalNodesProbabilities;
    public readonly double TerminalNodesProbability;
    public TreeChromosome(TreeNode rootNode, double terminalNodesProbability,
            IReadOnlyDictionary<TreeNode, double> terminalNodesProbabilities,
            IReadOnlyDictionary<TreeNode, double> nonTerminalNodesProbabilities,
            int? seed = null)
    {
        this._rootNode = rootNode;
        this._seed = seed;
        this._rng = seed.HasValue ? new Random(seed.Value) : new Random();
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
        this.NonTerminalNodesProbabilities,
        this._seed
    );
    /// <summary>
    /// this method creates full tree with depth DefaultDepth by default by default.
    /// </summary>
    /// <returns>New full TreeChromosome</returns>
    public override TreeChromosome CreateNew()
    => this.Clone(
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
                return this._rng.Choose(
                        this.TerminalNodesProbabilities.Keys.ToArray()
                    )
                    .Clone(children: null);
            }
        
        // else
        lock (this)
        {
            if (this._rng.NextDouble() < this.TerminalNodesProbability)
            {
                // choose a terminal node
                return this._rng
                    .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray())
                    .Clone(children: null);
            }
            else
            {
                // choose non-terminal node
                TreeNode[] children = Enumerable.Range(0, TreeNode.ChildrenAmount)
                    .Select(_ => this.CreateNewTreeFull(depth - 1))
                    .ToArray();
                return this._rng
                    .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray())
                    .Clone(children);
            }
        }

    }
    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return $"{this.GetType()}[ {this._rootNode} ]";
    }
}