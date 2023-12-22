public class DummySelection : Selection<TreeChromosome>
{
    public DummySelection(int? seed): base(seed) {}
    public override Tuple<TreeChromosome, TreeChromosome> ChooseParents(IReadOnlyList<TreeChromosome> population)
        => new Tuple<TreeChromosome, TreeChromosome>(
            population[_rng.Next(population.Count)].Clone(),
            population[_rng.Next(population.Count)].Clone()
        );
}