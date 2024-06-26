public class DummySelection : Selection<CartesianChromosome>
{
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IReadOnlyList<CartesianChromosome> population)
        => new Tuple<CartesianChromosome, CartesianChromosome>(
            population[Random.Shared.Next(population.Count)].Clone(),
            population[Random.Shared.Next(population.Count)].Clone()
        );

    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IReadOnlyList<CartesianChromosome> population, IReadOnlyList<double> probabilities)
        => new Tuple<CartesianChromosome, CartesianChromosome>(
            population[Random.Shared.Next(population.Count)].Clone(),
            population[Random.Shared.Next(population.Count)].Clone()
        );
}