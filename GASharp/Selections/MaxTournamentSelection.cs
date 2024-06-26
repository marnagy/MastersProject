
public class MaxTournamentSelection<T> : Selection<T> where T: Chromosome<T>
{
    public readonly int Folds;
    public MaxTournamentSelection(int folds)
    {
        if (folds <= 0)
            throw new ArgumentOutOfRangeException("Folds cannot be non-positive number.");

        this.Folds = folds;
    }
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    => new Tuple<T,T>(Enumerable.Range(0, Folds)
        .Select(_ => Random.Shared.Choose(population))
        .MaxBy(ind => ind.Fitness),
        Enumerable.Range(0, Folds)
        .Select(_ => Random.Shared.Choose(population))
        .MaxBy(ind => ind.Fitness)
    );

    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population, IReadOnlyList<double> probabilities)
    => new Tuple<T,T>(Enumerable.Range(0, Folds)
        .Select(_ => Random.Shared.ChooseProbs(population, probabilities))
        .MaxBy(ind => ind.Fitness),
        Enumerable.Range(0, Folds)
        .Select(_ => Random.Shared.ChooseProbs(population, probabilities))
        .MaxBy(ind => ind.Fitness)
    );
}