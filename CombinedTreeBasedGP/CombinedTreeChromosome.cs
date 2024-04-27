using System.Reflection.Metadata.Ecma335;
using System.Text;

public class CombinedTreeChromosome : Chromosome<CombinedTreeChromosome>
{
    public TreeChromosome[] Subchromosomes;
    public const double DefaultFitness = -2d;
    public double Score { get; internal set; }
    public static int MaxThreads = 1;

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
    private CombinedTreeChromosome(TreeChromosome[] subchromosomes, double fitness, double score)
    {
        this.Subchromosomes = subchromosomes;
        //this.Fitness = CombinedTreeChromosome.DefaultFitness;
        this.Fitness = fitness;
        this.Score = score;
    }
    public CombinedTreeChromosome Clone(TreeChromosome[] subchromosomes)
    => new CombinedTreeChromosome(
        subchromosomes
            .Select(subchrom => subchrom.Clone())
            .ToArray(),
        this.Fitness,
        this.Score
    );
    public override CombinedTreeChromosome Clone(bool preserveFitness = false)
    => this.Clone(this.Subchromosomes);

    public override CombinedTreeChromosome CreateNew()
    => new CombinedTreeChromosome(
        this.Subchromosomes
            .Select(subchrom => subchrom.CreateNew())
            .ToArray(),
        fitness: -1d,
        score: -1d
    );
    public double GetDepth()
    => this.Subchromosomes
        .Select(subchrom => subchrom.GetDepth())
        .Max();

    public IEnumerable<double> ComputeResults()
    {
        if (CombinedTreeChromosome.MaxThreads == 1)
        {
            return this.Subchromosomes
                .Select(subchrom => subchrom.ComputeResult());
        }
        else
        {
            return this.Subchromosomes
                .AsParallel().WithDegreeOfParallelism(CombinedTreeChromosome.MaxThreads)
                .Select(subchrom => subchrom.ComputeResult());
        }
    }
    // => this.Subchromosomes
    //     .AsParallel().WithDegreeOfParallelism(CombinedTreeChromosome.MaxThreads)
    //     .Select(subchrom => subchrom.ComputeResult());
    public double[] GetProbabilities()
    {
        var results = this.ComputeResults()
            .Select(value => Math.Exp(value))
            .ToArray();
        double summed = results.Sum();
        return results
            .Select(value => value / summed)
            .ToArray();
    }
    public override bool IsValid()
    => this.Subchromosomes
        .All(subchrom => subchrom.IsValid());

    internal string GetRepresentation()
    {
        var sb = new StringBuilder();
        foreach (var subchromosome in this.Subchromosomes)
        {
            sb.AppendLine(subchromosome.GetRepresentation());
        }
        return sb.ToString();
    }
    // => string.Join(
    //     '\n',
    //     this.Subchromosomes
    //         .Select(subchrom => subchrom.GetRepresentation())
    // );

    internal IEnumerable<double> ComputeResult()
    => this.Subchromosomes
        .Select(subchrom => subchrom.ComputeResult());
}