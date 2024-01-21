using System.Text;

public class CombinedTreeChromosome : Chromosome<CombinedTreeChromosome>
{
    public TreeChromosome[] Subchromosomes;
    public const double DefaultFitness = -2d;

    private CombinedTreeChromosome(int outputClasses, TreeChromosome[] newSubchromosomes)
    {
        this.Subchromosomes = newSubchromosomes;
        this.Fitness = CombinedTreeChromosome.DefaultFitness;
    }
    public static CombinedTreeChromosome CreateNew(int outputClasses, Func<TreeChromosome> newSubchromosome)
    => new CombinedTreeChromosome(
        outputClasses,
        Enumerable.Range(0, outputClasses)
            .Select(_ => newSubchromosome())
            .ToArray()
    );
    private CombinedTreeChromosome(TreeChromosome[] subchromosomes)
    {
        this.Subchromosomes = subchromosomes;
        this.Fitness = CombinedTreeChromosome.DefaultFitness;
    }
    public CombinedTreeChromosome Clone(TreeChromosome[] subchromosomes)
    => new CombinedTreeChromosome(
        subchromosomes
            .Select(subchrom => subchrom.Clone())
            .ToArray()
    );
    public override CombinedTreeChromosome Clone()
    => this.Clone(this.Subchromosomes);

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

    internal string GetRepresentation()
    => string.Join(
        '\n',
        this.Subchromosomes
            .Select(subchrom => subchrom.GetRepresentation())
    );

    internal IEnumerable<double> ComputeResult()
    => this.Subchromosomes
        .Select(subchrom => subchrom.ComputeResult());
}