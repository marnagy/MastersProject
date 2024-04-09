using System.Text;

public class TreeChromosome : Chromosome<TreeChromosome>
{
    public const double DefaultFitness = -1d;
    public static int DefaultDepth = 4;
    public static int DefaultSubtreeDepth = 2;
    private readonly int? _seed;
    public TreeNodeMaster RootNode;
    public readonly IReadOnlyDictionary<NodeFunctionality, double> TerminalNodesProbabilities;
    public readonly IReadOnlyDictionary<NodeFunctionality, double> NonTerminalNodesProbabilities;
    public readonly double TerminalNodesProbability;
    public TreeChromosome(TreeNodeMaster rootNode, double terminalNodesProbability,
            IReadOnlyDictionary<NodeFunctionality, double> terminalNodesProbabilities,
            IReadOnlyDictionary<NodeFunctionality, double> nonTerminalNodesProbabilities)
    {
        this.RootNode = rootNode;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.Fitness = TreeChromosome.DefaultFitness;
    }
    public override TreeChromosome Clone(bool _preserveFitness = false)
    => this.Clone(this.RootNode);
    public TreeChromosome Clone(TreeNodeMaster rootNode)
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
    public TreeNodeMaster CreateNewTreeFull(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depth);

        if (depth == 1)
            return new TreeNodeMaster(
                Random.Shared.Choose(
                   this.TerminalNodesProbabilities.Keys.ToArray()
                ),
                children: null
            );
        
        // choose non-terminal node
        return new TreeNodeMaster(
            Random.Shared
                .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray()),
            children: Enumerable.Range(0, TreeNodeMaster.ChildrenAmount)
                .Select(_ => this.CreateNewTreeGrow(depth - 1))
                .ToArray()
        );
    }
    /// <summary>
    /// Recursive function that creates tertiary tree with max depth of argument depth.
    /// </summary>
    /// <param name="depth">Maximum depth of Tree. <b>Root node is considered a layer 1.</b></param>
    /// <returns></returns>
    public TreeNodeMaster CreateNewTreeGrow(int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depth);

        if (depth == 1)
            return new TreeNodeMaster(
                Random.Shared.Choose(
                   this.TerminalNodesProbabilities.Keys.ToArray()
                ),
                children: null
            );

        // else
        if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
        {
            // choose a terminal node
            return new TreeNodeMaster(
                Random.Shared.Choose(
                   this.TerminalNodesProbabilities.Keys.ToArray()
                ),
                children: null
            );
        }
        else
        {
            // choose non-terminal node
            return new TreeNodeMaster(
                Random.Shared
                    .Choose(this.NonTerminalNodesProbabilities.Keys.ToArray()),
                children: Enumerable.Range(0, TreeNodeMaster.ChildrenAmount)
                    .Select(_ => this.CreateNewTreeGrow(depth - 1))
                    .ToArray()
            );
        }
    }
    /// <summary>
    /// Expects InputNodes to be set accordingly to 1 input
    /// </summary>
    /// <returns></returns>
    public double ComputeResult()
    => this.RootNode.Compute();
    public int GetDepth()
    => this.GetDepth(this.RootNode);
    public int GetDepth(TreeNodeMaster node)
    {
        if (node.Functionality.Arity == 0)
            return 1;
        
        return 1 + node.Children[..node.Functionality.Arity]
            .Select(childNode => this.GetDepth(childNode))
            .Max();
    }
    public override bool IsValid()
    => TreeChromosome.IsValid(this.RootNode);
    private static bool IsValid(TreeNodeMaster node)
    {
        if (node.Functionality is ValueFunctionality)
        {
            return !node.HasChildren;
        }

        return
            node.HasChildren &&
                node.Children
                    .All(childNode => TreeChromosome.IsValid(childNode));
    }
    public string GetRepresentation()
    {
        var sb = new StringBuilder();
        this.RootNode.GetRepresentation(sb);
        return sb.ToString();
    }
    public override string ToString()
    {
        return $"{this.GetType()}[ {this.RootNode} ]";
    }
}