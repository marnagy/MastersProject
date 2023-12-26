public class DummySelection : Selection<TreeChromosome>
{
    public override Tuple<TreeChromosome, TreeChromosome> ChooseParents(IReadOnlyList<TreeChromosome> population)
        => new Tuple<TreeChromosome, TreeChromosome>(
            population[Random.Shared.Next(population.Count)].Clone(),
            population[Random.Shared.Next(population.Count)].Clone()
        );
}