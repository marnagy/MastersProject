public class ShuffleChildrenMutation : Mutation<TreeChromosome>
{
    public readonly double PercentageToChange;
    public readonly IReadOnlyDictionary<TreeNodeMaster, double> TerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNodeMaster> TerminalNodes;
    public readonly IReadOnlyDictionary<TreeNodeMaster, double> NonTerminalNodesProbabilities;
    public readonly IReadOnlyList<TreeNodeMaster> NonTerminalNodes;
    public readonly double TerminalNodesProbability;
    public ShuffleChildrenMutation(double probability, 
            double percentageToChange): base(probability)
    {
        this.PercentageToChange = percentageToChange;
    }
    private bool ShouldChange()
    => Random.Shared.NextDouble() < this.MutationProbability;
    public override TreeChromosome Mutate(TreeChromosome ind, int genNum)
    {
        double rand_value = Random.Shared.NextDouble();

        if (rand_value < this.MutationProbability)
            this.Mutate(ind.RootNode, ind, genNum);

        return ind;
    }
    private void Mutate(TreeNodeMaster origNode, TreeChromosome ind, int genNum)
    {
        if (origNode.Functionality.Arity > 0)
        {
            if (Random.Shared.NextDouble() < this.PercentageToChange)
            {
                Random.Shared.Shuffle(origNode.Children);
            }

            foreach (TreeNodeMaster childNode in origNode.Children)
            {
                this.Mutate(childNode, ind, genNum);
            }
        }
    }
}