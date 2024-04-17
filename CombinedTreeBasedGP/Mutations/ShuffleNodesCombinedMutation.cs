public class ShuffleChildrenCombinedMutation : Mutation<CombinedTreeChromosome>
{
    private readonly Mutation<TreeChromosome> shuffleMutation;
    public ShuffleChildrenCombinedMutation(double probability, double percentageToChange): base(probability)
    {
        this.shuffleMutation = new ShuffleChildrenMutation(
            probability,
            percentageToChange
        );
    }
    public override CombinedTreeChromosome Mutate(CombinedTreeChromosome ind, int genNum)
    {
        foreach (var subchrom in ind.Subchromosomes)
        {
            this.shuffleMutation.Mutate(subchrom, genNum);
        }
        return ind;
    }
}