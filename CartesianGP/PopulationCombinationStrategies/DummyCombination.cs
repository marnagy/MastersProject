public class DummyCombination : PopulationCombinationStrategy<CartesianChromosome>
{
    public override CartesianChromosome[] Combine(CartesianChromosome[] oldPopulation, CartesianChromosome[] newPopulation)
        => newPopulation;
}