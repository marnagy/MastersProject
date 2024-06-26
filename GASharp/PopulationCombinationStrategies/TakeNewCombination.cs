public class TakeNewCombination<T> : PopulationCombinationStrategy<T> where T: Chromosome<T>
{
    public override T[] Combine(T[] oldPopulation, T[] newPopulation)
        => newPopulation
            .Select(ind => ind.Clone(preserveFitness: true))
            .ToArray();
}