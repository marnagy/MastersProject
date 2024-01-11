public class SwitchChildrenMutation : Mutation<TreeChromosome>
{
    public readonly double PercentageToChange;
    public readonly IReadOnlyDictionary<TreeNodeMaster, double> TerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNodeMaster> TerminalNodes;
    public readonly IReadOnlyDictionary<TreeNodeMaster, double> NonTerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNodeMaster> NonTerminalNodes;
    public readonly double TerminalNodesProbability;
    private const int DefaultNewDepth = 2;
    public SwitchChildrenMutation(double probability, 
            double percentageToChange): base(probability)
    {
        this.PercentageToChange = percentageToChange;
    }
    private bool ShouldChange()
    => Random.Shared.NextDouble() < this.MutationProbability;
    public override TreeChromosome Mutate(TreeChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        if (rand_value < this.MutationProbability)
            return ind.Clone();

        return ind.Clone(
            this.Mutate(ind.RootNode, ind, genNum)
        );
    }
    private TreeNodeMaster Mutate(TreeNodeMaster origNode, TreeChromosome ind, int genNum)
    {
        if (!(Random.Shared.NextDouble() < this.PercentageToChange))
            return origNode.Clone(
                origNode.Children?
                    .Select(childNode => this.Mutate(childNode, ind, genNum))
                    .ToArray()
            );
        
        var childrenArr = origNode.Children?
            .Select(childNode => childNode.Clone())
            .ToArray();

        if (childrenArr != null)
            Random.Shared.Shuffle(childrenArr);

        return origNode.Clone(
            childrenArr
        );
    }
}