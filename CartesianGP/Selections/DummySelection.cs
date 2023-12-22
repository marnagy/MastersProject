public class DummySelection : Selection<CartesianChromosome>
{
    public DummySelection(int? seed = null): base(seed) {}
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IReadOnlyList<CartesianChromosome> population)
        => new Tuple<CartesianChromosome, CartesianChromosome>(
            population[_rng.Next(population.Count)].Clone(),
            population[_rng.Next(population.Count)].Clone()
        );
}