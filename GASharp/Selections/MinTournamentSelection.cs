
public class MinTournamentSelection<T> : Selection<T> where T: Chromosome<T>
{
    public readonly int Folds;
    public MinTournamentSelection(int folds)
    {
        if (folds <= 0)
            throw new ArgumentOutOfRangeException("Folds cannot be non-positive number.");

        this.Folds = folds;
    }
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    => new Tuple<T,T>(
        this.ChooseParent(population).Clone(),
        this.ChooseParent(population).Clone()
    );

    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population, IReadOnlyList<double> probabilities)
    => new Tuple<T,T>(
        this.ChooseParent(population, probabilities).Clone(),
        this.ChooseParent(population, probabilities).Clone()
    );

    private T ChooseParent(IReadOnlyList<T> population)
    => Enumerable.Range(0, this.Folds)
        .Select(_ => Random.Shared.Choose(population)
        )
        .MinBy(ind => ind.Fitness);
    private T ChooseParent(IReadOnlyList<T> population, IReadOnlyList<double> probabilities)
    => Enumerable.Range(0, this.Folds)
        .Select(_ => Random.Shared.ChooseProbs(population, probabilities)
        )
        .MinBy(ind => ind.Fitness);
}