public class CombinedTreeChromosome : Chromosome<CombinedTreeChromosome>
{
    public TreeChromosome[] Subchromosomes;
    public CombinedTreeChromosome(int outputClasses, Func<TreeChromosome> newSubchromosome)
    {
        this.Subchromosomes = Enumerable.Range(0, outputClasses)
            .Select(_ => newSubchromosome())
            .ToArray();
    }
    private CombinedTreeChromosome(TreeChromosome[] subchromosomes)
    {
        this.Subchromosomes = subchromosomes;
    }
    public CombinedTreeChromosome Clone(TreeChromosome[] subchromosomes)
    => new CombinedTreeChromosome(subchromosomes);
    public override CombinedTreeChromosome Clone()
    => this.Clone(
        this.Subchromosomes
            .Select(subchrom => subchrom.Clone())
            .ToArray()
    );

    public override CombinedTreeChromosome CreateNew()
    => new CombinedTreeChromosome(
        this.Subchromosomes
            .Select(subchrom => subchrom.CreateNew())
            .ToArray()
    );
    public int GetDepth()
    => this.Subchromosomes.Max(subchrom => subchrom.GetDepth());

    public IEnumerable<double> ComputeResults()
    => this.Subchromosomes
        .Select(subchrom => subchrom.ComputeResult());

    public override bool IsValid()
    => this.Subchromosomes
        .All(subchrom => subchrom.IsValid());
}