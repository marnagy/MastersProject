public class DummySelection : Selection<CartesianChromosome>
{
    private Random _rng = new Random();
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IList<CartesianChromosome> population)
        => new Tuple<CartesianChromosome, CartesianChromosome>(
            population[_rng.Next(population.Count)].Clone(),
            population[_rng.Next(population.Count)].Clone()
        );
}